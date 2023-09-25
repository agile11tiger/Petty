namespace Petty.Services.Local.Speech
{
    public class PunctuationRecognizerService
    {
        public PunctuationRecognizerService(SpeechRecognizerService speechRecognizerService)
        {
            _speechRecognizerService = speechRecognizerService;
        }

        private static bool _isStarting;
        private static readonly object _locker = new();
        private readonly SpeechRecognizerService _speechRecognizerService;

        public event Action<string> BroadcastSpeechWithPunctuation;

        public void Start()
        {
            lock (_locker)
            {
                if (!_isStarting)
                {
                    _isStarting = true;
                }
            }
        }

        public void Stop()
        {
            lock (_locker)
            {
                if (BroadcastSpeechWithPunctuation.GetInvocationList().Length <= 1)
                {
                    _isStarting = false;
                }
            }
        }

    }
}
