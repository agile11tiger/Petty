using Petty.Helpers;
using Petty.ViewModels.Base;
namespace Petty.ViewModels.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel()
    {
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
