using Petty.Helpres;
using Petty.ViewModels.Base;

namespace Petty.ViewModels.Settings
{
    public partial class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(
            LoggerService loggerService,
            NavigationService navigationService,
            LocalizationService localizationService)
            : base(loggerService, navigationService, localizationService)
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
