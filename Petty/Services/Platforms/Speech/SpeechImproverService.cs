using Petty.MessengerCommands.FromPettyGuard;
using System.Text;
namespace Petty.Services.Platforms.Speech;

public class SpeechImproverService(LoggerService _loggerService, SpeechCommandRecognizerService _speechCommandRecognizer)
{
    private readonly Speech _speech = [];

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
