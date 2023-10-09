using Petty.Extensions;
using Petty.Resources.Localization;
using Petty.ViewModels.Base;

namespace Petty.ViewModels.Settings
{
    public partial class BaseSettingsViewModel : ViewModelBase
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
            _languages = _localizationService.SupportedCultures.Values.ToList();
            _informationPerceptionModes = new()
            {
                //carefully, sequence from PettySQLite.Models.InformationPerceptionModes
                new InformationPerceptionMode() { Id = 0, ModeName = $"0. {AppResources.InformationPerceptionModeReadOrVoice}" },
                new InformationPerceptionMode() { Id = 1, ModeName = $"1. {AppResources.InformationPerceptionModeRead}" },
                new InformationPerceptionMode() { Id = 2, ModeName = $"2. {AppResources.InformationPerceptionModeVoice}" },
            };
        }

        private string _informationPerceptionMode;
        private readonly DatabaseService _databaseService;
        private readonly SettingsService _settingsService;
        [ObservableProperty] private List<Language> _languages;
        [ObservableProperty] private PettySQLite.Models.BaseSettings _tempBaseSettings;
        [ObservableProperty] private List<InformationPerceptionMode> _informationPerceptionModes;

        /// <summary>
        /// todo: formating not working https://github.com/dotnet/maui/issues/14854
        /// </summary>
        public string SelectedInformationPerceptionMode
        {
            get => _informationPerceptionMode;
            set => SetProperty(ref _informationPerceptionMode, value.Substring(3, value.IndexOf('(')));
        }

        [RelayCommand]
        private async Task ApplyDefaultSettings()
        {
            TempBaseSettings = new PettySQLite.Models.BaseSettings();
            await GoBackPageAsync();
        }

        [RelayCommand]
        private async Task ApplySettings()
        {
            await GoBackPageAsync();
        }

        private async Task GoBackPageAsync()
        {
            await _databaseService.CreateOrUpdateAsync(TempBaseSettings);
            var needSoftRestart = _settingsService.Settings.BaseSettings.LanguageType != TempBaseSettings.LanguageType;
            _settingsService.Settings.BaseSettings = TempBaseSettings;

            if (needSoftRestart)
                _localizationService.SetCulture(_localizationService.SupportedCultures[_settingsService.Settings.BaseSettings.LanguageType].CultureInfo, true);
            else
                await _navigationService.GoBackAsync();
        }

        public class InformationPerceptionMode
        {
            public int Id { get; set; }
            public string ModeName { get; set; }
        }
    }
}
