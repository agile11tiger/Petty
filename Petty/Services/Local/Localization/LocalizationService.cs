using CommunityToolkit.Mvvm.Messaging;
using Petty.Helpers;
using Petty.MessengerCommands.Application;
using Petty.Resources.Localization;
using System.Globalization;
namespace Petty.Services.Local.Localization;

public class LocalizationService(IMessenger _messenger) : Service
{
    public readonly Dictionary<string, Language> SupportedCultures = new()
    {
        { "en-US", new Language { Name = AppResources.LanguageEnglish, CultureInfo = new CultureInfo("en-US")} },
        { "ru-RU", new Language { Name = AppResources.LanguageRussian, CultureInfo = new CultureInfo("ru-RU")} }
    };

    public bool IsRussianLanguage => CultureInfo.CurrentCulture.Name == "ru-RU";

    public void SetCulture(CultureInfo cultureInfo, bool needSoftRestart = false)
    {
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
        Preferences.Default.Set(SharedPreferencesHelper.LANGUAGE, cultureInfo.Name);

        if (needSoftRestart)
            _messenger.Send<RestartApplication>(new RestartApplication() { CultureInfo = cultureInfo });
    }

    public string Get(string key, params object[] parameters)
    {
        var str = AppResources.ResourceManager.GetString(key);
        return string.Format(CultureInfo.InvariantCulture, str, parameters);
    }
}
