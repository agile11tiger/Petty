using Petty.Helpers;
namespace Petty.ViewModels.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    [RelayCommand]
    private void NavigatedTo(NavigatedToEventArgs args)
    {
        _appShellViewModel.Title = AppResources.PageSettings;
    }

    [RelayCommand]
    private async Task GoToBaseSettingsPageAsync()
    {
        HapticFeedback();
        await _navigationService.GoToAsync(RoutesHelper.BASE_SETTINGS);
    }

    [RelayCommand]
    private async Task GoToDiagnosticsPettyPageAsync()
    {
        HapticFeedback();
        await _navigationService.GoToAsync(RoutesHelper.DIAGNOSTICS);
    }

    [RelayCommand]
    private async Task GoToVoiceSettingsPageAsync()
    {
        HapticFeedback();
        await _navigationService.GoToAsync(RoutesHelper.VOICE_SETTINGS);
    }
}
