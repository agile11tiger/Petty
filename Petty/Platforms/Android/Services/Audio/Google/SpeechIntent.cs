using Android.Content;
using Android.Speech;
using System.Globalization;
namespace Petty.Platforms.Android.Services.Audio.Google;

internal static class SpeechIntent
{
    public static Intent CreateSpeechIntent(CultureInfo culture)
    {
        //var intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
        //var javaLocale = Java.Util.Locale.ForLanguageTag(culture.Name);
        //var temp = Java.Util.Locale.Default;

        var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);


        voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak now");

        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
        voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
        voiceIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, global::Android.App.Application.Context.PackageName);
        voiceIntent.PutExtra(RecognizerIntent.ExtraPartialResults, true);

        //intent.PutExtra(RecognizerIntent.ExtraLanguagePreference, javaLocale);
        //intent.PutExtra(RecognizerIntent.ExtraLanguage, javaLocale);
        //intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
        //intent.PutExtra(RecognizerIntent.ExtraCallingPackage, global::Android.App.Application.Context.PackageName);
        //intent.PutExtra(RecognizerIntent.ExtraPartialResults, true);
        return voiceIntent;
    }
}
