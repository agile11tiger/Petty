﻿using CommunityToolkit.Mvvm.Messaging;
using Petty.Services.Platforms.Audio;
using Petty.Resources.Localization;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;
using System.IO.Compression;
using Vosk;
using static Petty.Services.Platforms.Speech.VoskModelInfos;
using static Java.Util.Concurrent.Flow;
using Petty.MessengerCommands.FromPettyGuard;
using System.Diagnostics;

namespace Petty.Services.Platforms.Speech
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

        private bool isAcceptWaveform;
        private Model _recognizerModel;
        private VoskRecognizer _recognizer;
        private bool _isRecognizingFromDisk;
        private readonly IMessenger _messenger;
        private string speechCache = string.Empty;
        private readonly LoggerService _loggerService;
        private SpeechRecognizerResult _speechRecognizerResult;
        private readonly WebRequestsService _webRequestsService;
        private readonly AudioPlayerService _audioPlayerService;
        private readonly List<float> _silenceThresholds = new();
        private readonly static SemaphoreSlim _locker = new(1, 1);
        private readonly UserMessagesService _userMessagesService;
        private readonly AudioRecorderService _audioRecorderService;

        private event Action<SpeechRecognizerResult> BroadcastSpeech;

        /// <summary>
        /// Gets/sets a value indicating the signal threshold that determines silence.
        /// If the recorder is being over or under aggressive when detecting silence, 
        /// you can alter this value to achieve different results.
        /// </summary>
        /// <remarks>Defaults to .15.  Value should be between 0 and 1.</remarks>
        public float SilenceThreshold { get; private set; } = .03f;

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

            var voskModelInfo = GetModelInfo();

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
            if (isAcceptWaveform && CanSkipRecognize(recorderData))
                return;

            //Returns a value indicating whether the speech has ended or not.
            //Presumably, when receiving a bunch of zeros, that is, silence.
            isAcceptWaveform = _recognizer.AcceptWaveform(recorderData, recorderData.Length);
            _speechRecognizerResult = new SpeechRecognizerResult();

            if (isAcceptWaveform)
            {
                _speechRecognizerResult.Speech = _recognizer.Result();

                if (_speechRecognizerResult.Speech.Length == RESULT_EMPTY_MESSAGE_LENGTH)
                    return;

                _speechRecognizerResult.Speech = _speechRecognizerResult.Speech[RESULT_START_MESSAGE_INDEX..^3];
                _speechRecognizerResult.IsResultSpeech = true;
            }
            else
            {
                _speechRecognizerResult.Speech = _recognizer.PartialResult();

                if (_speechRecognizerResult.Speech.Length == PARTIAL_RESULT_EMPTY_MESSAGE_LENGTH)
                    return;

                _speechRecognizerResult.Speech = _speechRecognizerResult.Speech[PARTIAL_RESULT_START_MESSAGE_INDEX..^3];
                _speechRecognizerResult.IsPartialSpeech = true;
            }

            if (_speechRecognizerResult.Speech.EndsWith(AppResources.Point))
            {
                _speechRecognizerResult.Speech = _recognizer.FinalResult();
                _speechRecognizerResult.Speech = _speechRecognizerResult.Speech[RESULT_START_MESSAGE_INDEX..^3];
                _speechRecognizerResult.IsFinalSpeech = true;
                _speechRecognizerResult.IsPartialSpeech = false;
                BroadcastSpeech?.Invoke(_speechRecognizerResult);
                return;
            }

            //isAcceptWaveform mean that result() called and we should send result
            //We dont ignore result even if it's the same. Since we need to separate completed pieces
            if (isAcceptWaveform || !_speechRecognizerResult.Speech.Reverse().SequenceEqual(speechCache.Reverse()))
            {
                speechCache = _speechRecognizerResult.Speech;
                BroadcastSpeech?.Invoke(_speechRecognizerResult);
            }

            if (_speechRecognizerResult.Speech.EndsWith(AppResources.Petty, true, null) || _speechRecognizerResult.IsCommandRecognized)
            {
                _recognizer.Reset();
            }
        }

        private bool CanSkipRecognize(byte[] recorderData)
        {
            var level = AudioFunctions.CalculateLevel(recorderData);
            _silenceThresholds.Add(level);

            if (_silenceThresholds.Count > 180) //~1min
            {
                SilenceThreshold = _silenceThresholds.Average();
                _silenceThresholds.Clear();
            }

            if (level > SilenceThreshold) // skip silence bytes
                return false;

            return true;
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