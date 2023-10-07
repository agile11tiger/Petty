using Petty.Extensions;
using Petty.Services.Local;
using Petty.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.ViewModels.Settings
{
    public partial class BaseSettingsViewModel: ViewModelBase
    {
        public BaseSettingsViewModel(
            LoggerService loggerService,
            DatabaseService databaseService,
            SettingsService settingsService,
            NavigationService navigationService,
            LocalizationService localizationService)
            : base(loggerService, navigationService, localizationService)
        {
            _databaseService = databaseService;
            _settingsService = settingsService;
            _tempBaseSettings = settingsService.Settings.BaseSettings.CloneJson();
            _languagesDictionary = _localizationService.SupportedCultures.Values.ToList();
        }

        private readonly DatabaseService _databaseService;
        private readonly SettingsService _settingsService;
        [ObservableProperty] private List<Language> _languagesDictionary;
        [ObservableProperty] private PettySQLite.Models.BaseSettings _tempBaseSettings;
        
        [RelayCommand]
        private Task ApplyDefaultSettings()
        {
            return GoToMainPageAsync(new PettySQLite.Models.BaseSettings());
        }

        [RelayCommand]
        private async Task ApplySettings()
        {
            await GoToMainPageAsync(TempBaseSettings.CloneJson());
        }

        private async Task GoToMainPageAsync(PettySQLite.Models.BaseSettings tempBaseSettings)
        {
            await _databaseService.CreateOrUpdateAsync(tempBaseSettings);
            var needSoftRestart = _settingsService.Settings.BaseSettings.LanguageType != tempBaseSettings.LanguageType;
            _settingsService.Settings.BaseSettings = tempBaseSettings;

            if (needSoftRestart)
                _localizationService.SetCulture(_localizationService.SupportedCultures[_settingsService.Settings.BaseSettings.LanguageType].CultureInfo, true);
            else
                await _navigationService.PopToMainAsync();
        }
    }
}
