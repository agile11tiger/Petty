using Petty.Helpers;
using Petty.Resources.Localization;
namespace Petty.ViewModels.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    [RelayCommand]
    private void NavigatedTo(NavigatedToEventArgs args)
    {
        _appShellViewModel.Title = AppResources.PageVoiceSettings;
    }

    [RelayCommand]
    private async Task GoToBaseSettingsPageAsync()
    {
        await _navigationService.GoToAsync(RoutesHelper.BASE_SETTINGS);
    }

    [RelayCommand]
    private async Task GoToDiagnosticsPettyPageAsync()
    {
        await _navigationService.GoToAsync(RoutesHelper.DIAGNOSTICS);
    }

    [RelayCommand]
    private async Task GoToVoiceSettingsPageAsync()
    {
        await _navigationService.GoToAsync(RoutesHelper.VOICE_SETTINGS);
    }
}
