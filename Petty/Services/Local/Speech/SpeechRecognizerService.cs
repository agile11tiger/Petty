//using Android.Webkit;
//using Petty.Services.Local.Audio;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vosk;

//namespace Petty.Services.Local.Speech
//{
//    public class SpeechRecognizerService
//    {
//        public SpeechRecognizerService()
//        {
//            InitRecorder();
//            _player = new AudioPlayer();
//            InitRecognizer();
//            _dataPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
//            _modelPathNew = _dataPath + "/VoiceModel/";
//            _downLoadedFileName = MODEL_WEB_LINK.Substring(MODEL_WEB_LINK.LastIndexOf('/') + 1);
//            _archivePath = _dataPath + _downLoadedFileName;
//        }

//        private const string MODEL_WEB_LINK = "https://alphacephei.com/vosk/models/vosk-model-small-en-us-0.15.zip";
//        private string _downLoadedFileName;
//        private string _dataPath;
//        private bool _isRecording;
//        private AudioPlayer _player;
//        private string _modelPathNew;
//        private string _archivePath;
//        private Model _recognizerModel;
//        private VoskRecognizer _recognizer;
//        private bool _isRecognizingFromDisk;
//        private bool _isRecognizingRealtime;
//        private AudioRecorderService _recorderService;

//        void InitRecorder()
//        {
//            _recorderService = new AudioRecorderService
//            {
//                PreferredSampleRate = 16000,
//                StopRecordingOnSilence = false,
//                StopRecordingAfterTimeout = false
//            };
//        }

//        async void InitRecognizer()
//        {
//            await CheckModelFolder();
//            System.Threading.Thread LaunchAndShowRecogModule = new System.Threading.Thread(() =>
//            {
//                //var str = "['oh one two three', 'four five six', 'seven eight nine zero', '[unk]']";
//                _recognizerModel = new Vosk.Model(_modelPathNew);
//                _recognizer = new Vosk.VoskRecognizer(_recognizerModel, 16000);
//            });
//            LaunchAndShowRecogModule.Start();
//            while (LaunchAndShowRecogModule.IsAlive) await Task.Yield();
//            RecogFromDiskColdStack.IsVisible = true;
//            RecognizeRealtimeStack.IsVisible = true;
//            InitRecogLabel.IsVisible = false;
//        }

//        async Task CheckModelFolder()
//        {
//            bool isModelReady = true;
//            bool isZipLoaded = true;
//            if (!System.IO.Directory.Exists(_modelPathNew))
//            {
//                System.IO.Directory.CreateDirectory(_modelPathNew);
//                isZipLoaded = false;
//                isModelReady = false;
//            }
//            if (System.IO.Directory.GetDirectories(_modelPathNew).Length == 0)
//            {
//                isModelReady = false;
//            }
//            if (!System.IO.File.Exists(_archivePath))
//            {
//                isZipLoaded = false;
//            }
//            if (!isModelReady)
//            {
//                if (!isZipLoaded)
//                {
//                    bool IsPickedYes = await DisplayAlert("Downloading", "For start voice recognizing you need to download a voice modek (over 50MB)", "Download", "Quit");
//                    if (!IsPickedYes) System.Diagnostics.Process.GetCurrentProcess().Kill();
//                    await DownloadModel();
//                }
//                Unzip();
//                MoveToModelFolder();
//                DeleteArchive();
//            }
//        }

//        async Task DownloadModel()
//        {
//            try
//            {
//                using (var Client = new System.Net.WebClient())
//                {
//                    System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
//                    await Client.DownloadFileTaskAsync(new Uri(MODEL_WEB_LINK), _archivePath);
//                    while (Client.IsBusy) await Task.Yield();
//                    Client.Dispose();
//                }
//            }
//            catch (System.Net.WebException ex)
//            {
//                Button_Clicked(null, null);
//                await DisplayAlert("Fail...", ex.Message + "\n" + ex.InnerException + "\n" + ex.TargetSite, "Молодец");
//                throw ex;
//            }
//        }

//        void Unzip()
//        {
//            System.IO.Compression.ZipFile.ExtractToDirectory(_archivePath, _modelPathNew);
//        }

//        void MoveToModelFolder()
//        {
//            var ShellFolder = System.IO.Directory.GetDirectories(_modelPathNew)[0];
//            var TargetFolders = System.IO.Directory.GetDirectories(ShellFolder);
//            for (int i = 0; i < TargetFolders.Length; i++)
//            {
//                var info = new System.IO.DirectoryInfo(TargetFolders[i]);
//                info.MoveTo(_modelPathNew + info.Name);
//            }
//            string[] TargetFiles = System.IO.Directory.GetFiles(ShellFolder);
//            for (int i = 0; i < TargetFiles.Length; i++)
//            {
//                var info = new System.IO.FileInfo(TargetFiles[i]);
//                info.MoveTo(_modelPathNew + info.Name);
//            }
//            System.IO.Directory.Delete(ShellFolder, true);
//        }

//        private async void SwitchRecord(object sender, EventArgs e)
//        {
//            _isRecording = !_isRecording;
//            if (_isRecording)
//            {
//                (sender as Button).Text = "Stop Record";
//                await _recorderService.StartRecording();
//            }
//            else
//            {
//                (sender as Button).Text = "Start Record";
//                await _recorderService.StopRecording();
//            }
//        }

//        private void PlayRecord(object sender, EventArgs e)
//        {
//            _player.Play(_recorderService.FilePath);
//        }

//        private async void RecognizeFromDisk(object sender, EventArgs e)
//        {
//            if (_isRecognizingFromDisk) return;
//            _isRecognizingFromDisk = true;
//            (sender as Button).Text = "Recognizing in process...";
//            using (var Reader = System.IO.File.OpenRead(_recorderService.FilePath))
//            {
//                byte[] buffer = new byte[4096];
//                int bytesRead;
//                string Result = String.Empty;
//                while ((bytesRead = await Reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
//                {
//                    _recognizer.AcceptWaveform(buffer, bytesRead);
//                    Result = _recognizer.PartialResult();
//                    if (Result.Length > 20)
//                    {
//                        Result = Result.Substring(17, Result.Length - 20);
//                        RecognizeFromDiskResult.Text = Result;
//                    }
//                    else
//                    {
//                        RecognizeFromDiskResult.Text = String.Empty;
//                    }
//                }
//                Result = _recognizer.FinalResult();
//                if (Result.Length > 20) Result = Result.Substring(14, Result.Length - 17);
//                RecognizeFromDiskResult.Text = Result;
//                Reader.Close();
//                Reader.Dispose();
//            }
//            (sender as Button).Text = "Recognize recorded sample";
//            _isRecognizingFromDisk = false;
//        }

//        void RecognizeRealtime(object sender, EventArgs e)
//        {
//            _isRecognizingRealtime = !_isRecognizingRealtime;
//            if (_isRecognizingRealtime)
//            {
//                StartRecognizeCycle();
//                (sender as Button).Text = "Stop Recognize";
//            }
//            else
//            {
//                (sender as Button).Text = "Start Recognize";
//            }
//        }

//        async void StartRecognizeCycle()
//        {
//            byte[] buffer = new byte[4096];
//            int bytesRead;
//            string Result;
//            await _recorderService.StartRecording();
//            var Reader = _recorderService.GetAudioFileStream();
//            while (_isRecognizingRealtime)
//            {
//                bytesRead = await Reader.ReadAsync(buffer, 0, buffer.Length);
//                _recognizer.AcceptWaveform(buffer, bytesRead);
//                Result = _recognizer.PartialResult();
//                if (Result.Length > 20)
//                {
//                    Result = Result.Substring(17, Result.Length - 20);
//                    RecognizeRealtimeResult.Text = Result;
//                }
//                else
//                {
//                    RecognizeRealtimeResult.Text = String.Empty;
//                }
//            }
//            Result = _recognizer.FinalResult();
//            if (Result.Length > 20) Result = Result.Substring(14, Result.Length - 17);
//            RecognizeRealtimeResult.Text = Result;
//            await _recorderService.StopRecording();
//            Reader.Close();
//            Reader.Dispose();
//        }
//    }
//}
