using Petty.Services.Platforms.Speech;

namespace Petty.MessengerCommands.FromPettyGuard
{
    public class SpeechRecognizerResult
    {
        public  string Speech { get; set; }
        public bool IsFinalSpeech { get; set; }
        public bool IsResultSpeech { get; set; }
        public bool IsPartialSpeech { get; set; }
        public bool IsCommandRecognized { get; set; }
    }
}
