using Petty.Extensions;
using Petty.Resources.Localization;
using Petty.ViewModels.DisplayAlert;
namespace Petty.ViewModels.Settings;

public partial class BaseSettingsViewModel : ViewModelBase
{
    public BaseSettingsViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        _tempBaseSettings = _settingsService.Settings.BaseSettings.CloneJson();
        var languagesListNumber = 0;
        _languages = _localizationService.SupportedCultures.Values
            .Select(lang =>
            {
                if (lang.CultureInfo.Name == _tempBaseSettings.LanguageType)
                    _pickerLanguageSelectedItemName = lang.Name;

                return new PickerLink(lang.Name, languagesListNumber++);
            }).ToList();
        //carefully, sequence from PettySQLite.Models.Modes
        _informationPerceptionModes =
        [
            new PickerLink($"{AppResources.BaseSettingsModeReadOrVoice} — {AppResources.BaseSettingsModeReadOrVoiceDescription}", 0, AppResources.BaseSettingsModeReadOrVoice),
            new PickerLink($"{AppResources.BaseSettingsModeRead} — {AppResources.BaseSettingsModeReadDescription}", 1, AppResources.BaseSettingsModeRead),
            new PickerLink($"{AppResources.BaseSettingsModeVoice} — {AppResources.BaseSettingsModeVoiceDescription}", 2, AppResources.BaseSettingsModeVoice),
        ];
        _informationPerceptionModeSelectedItemName = _informationPerceptionModes[(int)_tempBaseSettings.InformationPerceptionMode].SelectedLinkText;
    }

    private bool _isHapticFeedback;
    private readonly DatabaseService _databaseService;
    [ObservableProperty] private List<PickerLink> _languages;
    [ObservableProperty] private string _pickerLanguageSelectedItemName;
    [ObservableProperty] private List<PickerLink> _informationPerceptionModes;
    [ObservableProperty] private string _informationPerceptionModeSelectedItemName;
    [ObservableProperty] private PettySQLite.Models.BaseSettings _tempBaseSettings;

    public bool IsHapticFeedback 
    { 
        get => TempBaseSettings.IsHapticFeedback;
        set 
        {
            SetProperty(ref _isHapticFeedback, value);
            TempBaseSettings.IsHapticFeedback = _isHapticFeedback;
        } 
    }

    [RelayCommand]
    private void NavigatedTo(NavigatedToEventArgs args)
    {
        _appShellViewModel.Title = AppResources.PageBaseSettings;
    }

    [RelayCommand]
    private void SetLanguage(ILink link)
    {
        if (link != null)
        {
            HapticFeedback();
            PickerLanguageSelectedItemName = (link as PickerLink).SelectedLinkText;
            TempBaseSettings.LanguageType = _localizationService.SupportedCultures.Values.ElementAt(link.Index).CultureInfo.Name;
        }
    }

    [RelayCommand]
    private void SetInformationPerceptionMode(ILink link)
    {
        if (link != null)
        {
            HapticFeedback();
            InformationPerceptionModeSelectedItemName = (link as PickerLink).SelectedLinkText;
            TempBaseSettings.InformationPerceptionMode = (InformationPerceptionModes)link.Index;
        }
    }

    [RelayCommand]
    private async Task ApplyDefaultSettingsAsync()
    {
        HapticFeedback();
        TempBaseSettings = new PettySQLite.Models.BaseSettings();
        await GoBackPageAsync();
    }

    [RelayCommand]
    private async Task ApplySettingsAsync()
    {
        HapticFeedback();
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
