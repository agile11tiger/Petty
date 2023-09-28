namespace Petty.MessengerCommands.FromPettyGuard
{
    public class SpeechRecognizerResult
    {
        private volatile string _speech;
        private volatile bool _isCommandRecognized;
        public  string Speech { get => _speech; set => _speech = value; }
        public bool IsCommandRecognized { get => _isCommandRecognized; set => _isCommandRecognized = value; }
    }
}
