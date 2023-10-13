using Petty.Helpres;
using Petty.ViewModels.Base;

namespace Petty.ViewModels.Settings
{
    public partial class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
        }

        [RelayCommand]
        private async Task GoToBaseSettingsPage()
        {
            await _navigationService.GoToAsync(RoutesHelper.BASE_SETTINGS);
        }

        [RelayCommand]
        private async Task GoToDiagnosticsPettyPage()
        {
            await _navigationService.GoToAsync(RoutesHelper.DIAGNOSTICS);
        }

        [RelayCommand]
        private async Task GoToVoiceSettingsPage()
        {
            await _navigationService.GoToAsync(RoutesHelper.VOICE_SETTINGS);
        }
    }
}
