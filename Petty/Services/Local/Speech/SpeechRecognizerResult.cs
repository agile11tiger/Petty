namespace Petty.Services.Local.Speech
{
    public struct SpeechRecognizerResult
    {
        public string Speech { get; set; }
        public bool IsPettyCommand { get; set; }
    }
}
