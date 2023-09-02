using Petty.Extensions;
using Petty.Resources.Localization;
using Petty.Services.Local.Localization;
using Petty.ViewModels.Base;
using System.Globalization;

namespace Petty.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(
            LoggerService loggerService,
            DatabaseService databaseService,
            SettingsService settingsService,
            NavigationService navigationService,
            LocalizationService localizationService)
            : base(loggerService, navigationService, localizationService)
        {
            _databaseService = databaseService;
            _settingsService = settingsService;
            _tempSettings = settingsService.Settings.CloneJson();
        }

        [ObservableProperty] private Settings _tempSettings;
        [ObservableProperty] private List<Language> _languagesDictionary = LocalizationService.SupportedCultures.Values.ToList();
        private readonly DatabaseService _databaseService;
        private readonly SettingsService _settingsService;

        [RelayCommand]
        private Task ApplyDefaultSettings()
        {
            return GoToMainPageAsync(new Settings());
        }

        [RelayCommand]
        private async Task ApplySettings()
        {
            await GoToMainPageAsync(TempSettings.CloneJson());
        }

        private async Task GoToMainPageAsync(Settings tempSettings)
        {
            await _databaseService.CreateOrUpdateAsync(tempSettings);
            var needSoftRestart = _settingsService.Settings.LanguageType != tempSettings.LanguageType;
            _settingsService.ApplySettings(tempSettings);

            if (needSoftRestart)
                LocalizationService.SetCulture(LocalizationService.SupportedCultures[_settingsService.Settings.LanguageType].CultureInfo, true);
            else
                await NavigationService.PopToMainAsync();
        }
    }
}
