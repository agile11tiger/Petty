using CommunityToolkit.Mvvm.Messaging;
using Petty.Services.Platforms.Audio;
using Petty.Resources.Localization;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;
using System.IO.Compression;
using Vosk;
using static Petty.Services.Local.Speech.VoskModelInfos;
using static Java.Util.Concurrent.Flow;

namespace Petty.Services.Local.Speech
{
    public class SpeechRecognizerService : IDisposable
    {
        public SpeechRecognizerService(
            IMessenger messenger,
            LoggerService loggerService,
            WebRequestsService webRequestsService,
            AudioPlayerService audioPlayerService,
            UserMessagesService userMessagesService,
            AudioRecorderService audioRecorderService)
        {
            _messenger = messenger;
            _loggerService = loggerService;
            _webRequestsService = webRequestsService;
            _audioPlayerService = audioPlayerService;
            _userMessagesService = userMessagesService;
            _audioRecorderService = audioRecorderService;
        }

        private const int RESULT_START_MESSAGE_INDEX = 14;
        private const int RESULT_EMPTY_MESSAGE_LENGTH = 17;// "{\n  \"text\" : \"\"\n}"
        private const int PARTIAL_RESULT_START_MESSAGE_INDEX = 17;
        private const int PARTIAL_RESULT_EMPTY_MESSAGE_LENGTH = 20;// "{\n  \"partial\" : \"\"\n}
        private const string REALTIME_DATA_FILE_PATH = "speechRecognizerRealtimeData.wav";

        private Model _recognizerModel;
        private VoskRecognizer _recognizer;
        private bool _isRecognizingFromDisk;
        private readonly IMessenger _messenger;
        private readonly LoggerService _loggerService;
        private static SemaphoreSlim _locker = new(1, 1);
        private readonly WebRequestsService _webRequestsService;
        private readonly AudioPlayerService _audioPlayerService;
        private readonly UserMessagesService _userMessagesService;
        private readonly AudioRecorderService _audioRecorderService;

        private event Action<SpeechRecognizerResult> BroadcastSpeech;

        /// <summary>
        /// Try start the speech recognizer.
        /// </summary>
        public async Task<bool> TryStartAsync(Action<SpeechRecognizerResult> subscriber)
        {
            await _locker.WaitAsync();

            try
            {
                if (BroadcastSpeech == null)
                {
                    if (!await TryInitializeRecognizer())
                        return false;

                    _audioRecorderService.StartRecording(OnBroadcastAudioRecorderData);
                }

                BroadcastSpeech += subscriber;
                return true;
            }
            finally { _locker.Release(); }
        }

        /// <summary>
        /// Stop the speech recognizer.
        /// </summary>
        public async Task StopAsync(Action<SpeechRecognizerResult> subscriber)
        {
            await _locker.WaitAsync();

            try
            {
                if (BroadcastSpeech.GetInvocationList().Length == 1)
                    _audioRecorderService.StopRecording(OnBroadcastAudioRecorderData);

                BroadcastSpeech -= subscriber;
            }
            finally { _locker.Release(); }
        }

        public async Task RecognizeFromDiskAsync(Action<string, bool> updateResult, string filePath)
        {
            if (_isRecognizingFromDisk)
                await _userMessagesService.SendMessageAsync(AppResources.TryLater, AppResources.Ok);

            if (await TryInitializeRecognizer())
            {
                _isRecognizingFromDisk = true;
                using var reader = File.OpenRead(filePath);
                await Recognize(reader, updateResult);
                _isRecognizingFromDisk = false;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private async Task<bool> TryInitializeRecognizer()
        {
            if (_recognizer != null)
                return true;

            var voskModelInfo = VoskModelInfos.GetModelInfo();

            try
            {
                if (await CheckModelFolderAsync(voskModelInfo))
                {
                    _recognizerModel = new Model(voskModelInfo.Path);
                    _recognizer = new VoskRecognizer(_recognizerModel, _audioRecorderService.PreferredSampleRate);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
            }

            return false;
        }

        private async Task<bool> CheckModelFolderAsync(VoskModelInfo voskModelInfo)
        {
            CheckAndDeleteIfCorruptedFiles(voskModelInfo);

            if (!Directory.Exists(voskModelInfo.Path))
            {
                if (Connectivity.Current.NetworkAccess == NetworkAccess.None)
                    return await _userMessagesService.SendRequestAsync(AppResources.CheckNetworkConnection, AppResources.Ok);

                if (!await _userMessagesService.SendRequestAsync(AppResources.DownloadVoskModelMessage, AppResources.Later, AppResources.Download, AppResources.Downloading))
                    return false;

                await _webRequestsService.DownloadAsync(
                    new Uri(voskModelInfo.Uri),
                    voskModelInfo.ArchivePath,
                    (percentages) => _messenger.Send(new UpdateProgressBar() { Percentages = percentages }));
            }

            if (File.Exists(voskModelInfo.ArchivePath))
            {
                ZipFile.ExtractToDirectory(voskModelInfo.ArchivePath, voskModelInfo.DataPath);
                File.Delete(voskModelInfo.ArchivePath);
            }

            return true;
        }

        private void CheckAndDeleteIfCorruptedFiles(VoskModelInfo voskModelInfo)
        {
            if (File.Exists(voskModelInfo.ArchivePath))
            {
                var fileInfo = new FileInfo(voskModelInfo.ArchivePath);

                if (fileInfo.Length != voskModelInfo.ArchiveByteSize)
                    File.Delete(voskModelInfo.ArchivePath);
            }

            if (Directory.Exists(voskModelInfo.Path))
            {
                var directoryInfo = new DirectoryInfo(voskModelInfo.Path);
                var directoryLength = directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);

                if (directoryLength != voskModelInfo.ByteSize)
                    Directory.Delete(voskModelInfo.Path, true);
            }
        }

        private void OnBroadcastAudioRecorderData(byte[] recorderData)
        {
            _recognizer.AcceptWaveform(recorderData, recorderData.Length);
            var result = new SpeechRecognizerResult() { Speech = _recognizer.PartialResult() };

            if (result.Speech.Length > PARTIAL_RESULT_EMPTY_MESSAGE_LENGTH)
            {
                result.Speech = result.Speech[PARTIAL_RESULT_START_MESSAGE_INDEX..^3];

                if (result.Speech.EndsWith(AppResources.Petty, true, null))
                {
                    _recognizer.Reset();
                    _recognizer.AcceptWaveform(recorderData, recorderData.Length);
                }

                BroadcastSpeech.Invoke(result);

                if (result.IsPettyCommand)
                {
                    _recognizer.FinalResult();
                    return;
                }

                //Todo: do i need improve speed AppResources.Point???
                //If I switch the language, the constant string needs to be updated.
                //возможно, стоит добавить еще условия тишина в течение 2-3 сек,
                //тогда мы распознаем это как точка в предложение, а не слово точка.
                if (result.Speech.EndsWith(AppResources.Point))
                {
                    result.Speech = _recognizer.FinalResult();
                    result.Speech = result.Speech[RESULT_START_MESSAGE_INDEX..^3];
                    BroadcastSpeech?.Invoke(result);
                }
            }
        }

        private async Task Recognize(Stream reader, Action<string, bool> updateResult)
        {
            int bytesRead;
            string result;
            byte[] buffer = new byte[4096];

            while ((bytesRead = await reader.ReadAsync(buffer)) > 0)
            {
                _recognizer.AcceptWaveform(buffer, bytesRead);
                result = _recognizer.PartialResult();

                if (result.Length > PARTIAL_RESULT_EMPTY_MESSAGE_LENGTH)
                {
                    result = result[PARTIAL_RESULT_START_MESSAGE_INDEX..^3];
                    updateResult(result, false);
                }
            }

            result = _recognizer.FinalResult();

            if (result.Length > RESULT_EMPTY_MESSAGE_LENGTH)
            {
                result = result[RESULT_START_MESSAGE_INDEX..^3];
                updateResult(result, true);
            }
        }
    }
}
