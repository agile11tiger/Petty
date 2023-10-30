namespace SpeechEngine.Speech;

public class SpeechRecognizerResult
{
    public string Speech { get; set; }
    public bool IsFinalSpeech { get; set; }
    public bool IsResultSpeech { get; set; }
    public bool IsPartialSpeech { get; set; }
    public Action NotifyCommandRecognized { get; set; }

    public override string ToString()
    {
        return 
            $"{nameof(IsFinalSpeech)}:{IsFinalSpeech}\r\n" +
            $"{nameof(IsResultSpeech)}:{IsResultSpeech}\r\n" +
            $"{nameof(IsPartialSpeech)}:{IsPartialSpeech}\r\n" +
            $"Speech: {Speech}";
    }
}
