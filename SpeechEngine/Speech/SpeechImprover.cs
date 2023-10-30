using SpeechEngine.Services;
using System.Text;
namespace SpeechEngine.Speech;

public class SpeechImprover(ILoggerService _loggerService, SpeechCommandRecognizer _speechCommandRecognizer)
{
    private readonly SpeechWords _speech = [];

    public void Improve(SpeechRecognizerResult speechResult)
    {
        //добавить в настройках возможность для каждой команды Петти вначале?
        _speech.Add(speechResult);
        _speechCommandRecognizer.ApplyCommands(_speech);
        _speechCommandRecognizer.AddPunctuation(_speech);
        var speech = new StringBuilder();
        
        for (var i = 0; i < _speech.Length; i++)
        {
            var currentWord = _speech[i];

            if (currentWord == string.Empty)
                continue;

            while (char.TryParse(i + 1 < _speech.Length ? _speech[i + 1] : null, out char punctuationMark))
            {
                currentWord += punctuationMark;
                i++;
            }

            speech.Append(currentWord + " ");
        }

        _loggerService.Log(speech.ToString());
        speechResult.Speech = speech.ToString();
    }
}
