﻿using Petty.MessengerCommands.FromPettyGuard;
using Petty.Resources.Localization;
using Petty.Services.Local;
using System.Diagnostics;
using System.Linq;

namespace Petty.Services.Platforms.Speech
{
    public static class PunctuationRecognizer
    {
        public readonly static Dictionary<string, string> Punctuations = new()
        {
            { AppResources.Point, "." },
            { AppResources.Comma, "," },
            { AppResources.Colon, ":" },
            { AppResources.Quote, "\"" },
            { AppResources.NewLine, "\r\n" },
            { AppResources.QuestionMark, "?" },
            { AppResources.ExclamationMark, "!" },
        };

        public static string AddPunctuation(this string text)
        {
            var textArray = text.Split(' ');

            for (var i = 0; i < textArray.Length; i++)
            {
                if (Punctuations.TryGetValue(textArray[i], out string punctuationMark))
                {
                    if (textArray.Length == 1)
                        textArray[i] = punctuationMark;
                    else
                    {
                        //example: point back
                        if (i == 0)
                            textArray[i] = punctuationMark;
                        else
                            textArray[i - 1] += punctuationMark;

                        textArray[i] = string.Empty;
                    }
                }

                if (textArray.Length > 1 && i > 1 && Punctuations.TryGetValue($"{textArray[i - 1]} {textArray[i]}", out punctuationMark))
                {
                    if (textArray.Length == 2)
                        textArray[i] = punctuationMark;
                    else
                    {
                        if (i == 1)
                            textArray[i] = punctuationMark;
                        else 
                            textArray[i - 2] += punctuationMark;

                        textArray[i - 1] = string.Empty;
                        textArray[i] = string.Empty;
                    }
                }
            }

            return string.Join(' ', textArray.Where(word => word != string.Empty));
        }
    }
}