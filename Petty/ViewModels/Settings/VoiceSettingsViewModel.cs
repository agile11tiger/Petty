using Petty.Extensions;
using Petty.Resources.Localization;
namespace Petty.ViewModels.Settings;

public partial class VoiceSettingsViewModel(
    VoiceService _voiceService, 
    DatabaseService _databaseService, 
    SettingsService _settingsService)
    : ViewModelBase
{
    private VoiceSettings _tempVoiceSettings = _settingsService.Settings.VoiceSettings.CloneJson();
    [ObservableProperty] private string _speech = AppResources.VoiceSettingsEditorVoiceTestSpeech;

    //https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device-media/text-to-speech?tabs=android#:~:text=Maximum-,Pitch,-0
    public float PitchMaxValue { get => 2; }
    public float VolumeMaxValue { get => 1; }
    public string PitchValueText { get => _localizationService.Get(nameof(AppResources.VoiceSettingsValue), PitchValue); }

    public float PitchValue
    {
        get => (float)Math.Round(_tempVoiceSettings.Pitch, 2);
        set
        {
            if (EqualityComparer<double>.Default.Equals(_tempVoiceSettings.Pitch, value))
                return;

            _tempVoiceSettings.Pitch = value;
            OnPropertyChanged(nameof(PitchValue));
            OnPropertyChanged(nameof(PitchValueText));
        }
    }

    public string VolumeValueText
    {
        get => _localizationService.Get(nameof(AppResources.VoiceSettingsValue), VolumeValue);
    }

    public float VolumeValue
    {
        get => (float)Math.Round(_tempVoiceSettings.Volume, 2);
        set
        {
            if (EqualityComparer<double>.Default.Equals(_tempVoiceSettings.Volume, value))
                return;

            _tempVoiceSettings.Volume = value;
            OnPropertyChanged(nameof(VolumeValue));
            OnPropertyChanged(nameof(VolumeValueText));
        }
    }

    [RelayCommand]
    private void NavigatedTo(NavigatedToEventArgs args)
    {
        _appShellViewModel.Title = AppResources.PageVoiceSettings;
    }

    [RelayCommand]
    private async Task SpeakAsync()
    {
        HapticFeedback();
        await _voiceService.SpeakAsync(Speech, _tempVoiceSettings);
    }

    [RelayCommand]
    private async Task ApplyDefaultSettingsAsync()
    {
        HapticFeedback();
        _tempVoiceSettings = new PettySQLite.Models.VoiceSettings();
        await GoBackAsync();
    }

    [RelayCommand]
    private async Task ApplySettingsAsync()
    {
        HapticFeedback();
        await GoBackAsync();
    }

    private async Task GoBackAsync()
    {
        await _databaseService.CreateOrUpdateAsync(_tempVoiceSettings);
        _settingsService.Settings.VoiceSettings = _tempVoiceSettings;
        await _navigationService.GoBackAsync();
    }
}
