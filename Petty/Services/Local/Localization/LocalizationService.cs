﻿using CommunityToolkit.Mvvm.Messaging;
using Petty.Helpres;
using Petty.Resources.Localization;
using Petty.WeakReferenceMessengerCommands;
using System.Globalization;

namespace Petty.Services.Local.Localization
{
    public class LocalizationService
    {
        public LocalizationService()
        {

        }

        public static Dictionary<string, Language> SupportedCultures = new()
        {
            { "en-US", new Language { Name = AppResources.English, CultureInfo = new CultureInfo("en-US")} },
            { "ru-RU", new Language { Name = AppResources.Russian, CultureInfo = new CultureInfo("ru-RU")} }
        };

        private Dictionary<string, string> _cacheStringWithParameters = new Dictionary<string, string>();

        public static void SetCulture(CultureInfo cultureInfo, bool needSoftRestart = false)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            Preferences.Default.Set(PreferencesHelper.LANGUAGE, cultureInfo.Name);

            if (needSoftRestart)
                WeakReferenceMessenger.Default.Send<RestartCommand>(new RestartCommand() { CultureInfo = cultureInfo });
        }

        public static string Get(string key, params object[] parameters)
        {
            var str = AppResources.ResourceManager.GetString(key);
            return string.Format(CultureInfo.InvariantCulture, str, parameters);
        }
    }
}
