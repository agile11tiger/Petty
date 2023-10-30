namespace Petty.Services.Platforms.Speech;

public class SpeechCommandRecognizerService(NumberParsingService _numberParsingService)
{
    public const int REPEAT_COMMAND_MAX_COUNT = 99;
    public static Dictionary<string, string> Commands { get; } = new()
    {
        { AppResources.SpeechCommandClear, AppResources.SpeechCommandClearDecription },
        { AppResources.SpeechCommandDeleteWord, AppResources.SpeechCommandDeleteWordDecription },
    };
    public static Dictionary<string, string> Punctuations { get; } = new()
    {
        { AppResources.SpeechCommandDash, "—" },
        { AppResources.SpeechCommandPoint, "." },
        { AppResources.SpeechCommandComma, "," },
        { AppResources.SpeechCommandColon, ":" },
        { AppResources.SpeechCommandHyphen, "-" },
        { AppResources.SpeechCommandQuote, "\"" },
        { AppResources.SpeechCommandSemicolon, ";" },
        { AppResources.SpeechCommandNewLine, "\r\n" },
        { AppResources.SpeechCommandOpenBracket, "(" },
        { AppResources.SpeechCommandCloseBracket, ")" },
        { AppResources.SpeechCommandQuestionMark, "?" },
        { AppResources.SpeechCommandExclamationMark, "!" }
    };

    public void ApplyCommands(Speech speech)
    {
        for (var i = 0; i < speech.Length; i++)
        {
            if (speech[i] == AppResources.SpeechCommandClear)
            {
                speech.Clear();
                return;
            }

            if (speech[i] == AppResources.SpeechCommandDeleteWord)
            {
                speech[i] = string.Empty;
                var nextWord = speech[i + 1];

                if (_numberParsingService.TryParseNumber(nextWord, out var number))
                {
                    number = number > REPEAT_COMMAND_MAX_COUNT ? REPEAT_COMMAND_MAX_COUNT : number;

                    for (var j = 1; j <= number; j++)
                        speech[i - j] = string.Empty;
                }
                else
                    speech[i - 1] = string.Empty;
            }
        }
    }

    public void AddPunctuation(Speech speech)
    {
        for (var i = 0; i < speech.Length; i++)
        {
            if (Punctuations.TryGetValue(speech[i], out string punctuationMark)) //min one word example: point
            {
                if (i == 0) //just add
                    speech[i] = RepeatPunctuation(speech, ref i, punctuationMark);
                else
                {
                    //add to previous word
                    speech[i] = string.Empty;
                    speech[i - 1] += RepeatPunctuation(speech, ref i, punctuationMark);
                }
            }

            //min two word example: question mark
            if (i >= 1 && Punctuations.TryGetValue($"{speech[i - 1]} {speech[i]}", out punctuationMark))
            {
                speech[i] = string.Empty;
                speech[i - 1] = string.Empty;

                if (i < 2) //just add
                    speech[i - 1] = RepeatPunctuation(speech, ref i, punctuationMark);
                else //add to previous word
                    speech[i - 2] += RepeatPunctuation(speech, ref i, punctuationMark);
            }
        }
    }

    private string RepeatPunctuation(Speech speech, ref int i, string punctuationMark)
    {
        var punctuationMarks = punctuationMark;
        var nextWord = i + 1 < speech.Length ? speech[i + 1] : string.Empty;

        if (_numberParsingService.TryParseNumber(nextWord, out var number))
        {
            speech[i + 1] = string.Empty;
            number = number > REPEAT_COMMAND_MAX_COUNT ? REPEAT_COMMAND_MAX_COUNT : number;

            for (var j = 1; j < number; j++)
                punctuationMarks += punctuationMark;
        }

        return punctuationMarks;
    }
}
