using Android.Speech;
using System.Globalization;

namespace Petty.Platforms.Android.Audio.Google
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// https://github.com/math3ussdl/Plugin.Maui.AudioRecorder
    /// </remarks>
    public class GoogleSpeechRecognizer : Java.Lang.Object
    {
        public GoogleSpeechRecognizer()
        {
        }

        private SpeechRecognitionListener _listener;
        private SpeechRecognizer _speechRecognizer;

        public bool IsRecording { get; private set; }
        ///<Summary>
        /// Path where the file was save
        ///</Summary>
        public string FilePath { get; set; } = Path.Combine(Path.GetTempPath(), "lol.wav");

        /// <summary>
        /// Starts a new recording, and fills the recognitionResult with a text transcribed.
        /// </summary>
        public Task StartRecordAsync(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken)
        {
            var taskResult = new TaskCompletionSource<string>();
            _listener = new SpeechRecognitionListener
            {
                Error = ex => taskResult.TrySetException(new Exception("Speech recognition failed! " + ex)),
                PartialResults = recognitionResult.Report,
                Results = sentence => taskResult.TrySetResult(sentence)
            };

            _speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(global::Android.App.Application.Context);

            if (_speechRecognizer is null)
                throw new ArgumentException("Speech recognizer is not available!");

            _speechRecognizer.SetRecognitionListener(_listener);
            _speechRecognizer.StartListening(SpeechIntent.CreateSpeechIntent(culture));
            return Task.CompletedTask;
        }

        public void StopRecordAsync()
        {
            _speechRecognizer.StopListening();
            _speechRecognizer.Destroy();
        }
    }
}
