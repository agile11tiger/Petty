using Petty.Extensions;
using Petty.Resources.Localization;
using Petty.ViewModels.Base;
using Petty.ViewModels.DisplayAlert;
namespace Petty.ViewModels.Settings;

public partial class BaseSettingsViewModel : ViewModelBase
{
    public BaseSettingsViewModel(DatabaseService databaseService, SettingsService settingsService)
    {
        _databaseService = databaseService;
        _settingsService = settingsService;
        _tempBaseSettings = settingsService.Settings.BaseSettings.CloneJson();
        var languagesListNumber = 0;
        _languages = _localizationService.SupportedCultures.Values
            .Select(lang =>
            {
                if (lang.CultureInfo.Name == _tempBaseSettings.LanguageType)
                    _pickerLanguageSelectedItemName = lang.Name;

                return new PickerLink(lang.Name, languagesListNumber++);
            }).ToList();
        //carefully, sequence from PettySQLite.Models.InformationPerceptionModes
        _informationPerceptionModes =
        [
            new PickerLink($"{AppResources.InformationPerceptionModeReadOrVoice} — {AppResources.InformationPerceptionModeReadOrVoiceDescription}", 0, AppResources.InformationPerceptionModeReadOrVoice),
            new PickerLink($"{AppResources.InformationPerceptionModeRead} — {AppResources.InformationPerceptionModeReadDescription}", 1, AppResources.InformationPerceptionModeRead),
            new PickerLink($"{AppResources.InformationPerceptionModeVoice} — {AppResources.InformationPerceptionModeVoiceDescription}", 2, AppResources.InformationPerceptionModeVoice),
        ];
        _informationPerceptionModeSelectedItemName = _informationPerceptionModes[(int)_tempBaseSettings.InformationPerceptionMode].SelectedLinkText;
    }

    private readonly DatabaseService _databaseService;
    private readonly SettingsService _settingsService;
    [ObservableProperty] private List<PickerLink> _languages;
    [ObservableProperty] private string _pickerLanguageSelectedItemName;
    [ObservableProperty] private List<PickerLink> _informationPerceptionModes;
    [ObservableProperty] private string _informationPerceptionModeSelectedItemName;
    [ObservableProperty] private PettySQLite.Models.BaseSettings _tempBaseSettings;

    [RelayCommand]
    private void SetLanguage(ILink link)
    {
        if (link != null)
        {
            PickerLanguageSelectedItemName = (link as PickerLink).SelectedLinkText;
            TempBaseSettings.LanguageType = _localizationService.SupportedCultures.Values.ElementAt(link.Index).CultureInfo.Name;
        }
    }

    [RelayCommand]
    private void SetInformationPerceptionMode(ILink link)
    {
        if (link != null)
        {
            InformationPerceptionModeSelectedItemName = (link as PickerLink).SelectedLinkText;
            TempBaseSettings.InformationPerceptionMode = (InformationPerceptionModes)link.Index;
        }
    }

    [RelayCommand]
    private async Task ApplyDefaultSettingsAsync()
    {
        TempBaseSettings = new PettySQLite.Models.BaseSettings();
        await GoBackPageAsync();
    }

    [RelayCommand]
    private async Task ApplySettingsAsync()
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

        await _navigationService.GoBackAsync();
    }
}
