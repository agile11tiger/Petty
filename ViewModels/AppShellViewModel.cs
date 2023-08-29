using Petty.ViewModels.Base;
using Petty.ViewModels.Components;

namespace Petty.ViewModels
{
    public partial class AppShellViewModel : ViewModelBase
    {
        public AppShellViewModel(
            LoggerService loggerService,
            NavigationService navigationService,
            LocalizationService localizationService,
            RunningTextViewModel runningTextViewModel)
            : base(loggerService, navigationService, localizationService)
        {
            _runningTextViewModel = runningTextViewModel;
        }

        [ObservableProperty] private RunningTextViewModel _runningTextViewModel;
        [ObservableProperty] private bool _isAnimationPlayingCoffeeGif;

        [RelayCommand]
        private async Task StartCoffeGifAsync()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(3000);
                IsAnimationPlayingCoffeeGif = true;
            });
        }

        [RelayCommand]
        private void StopCoffeGif()
        {
            IsAnimationPlayingCoffeeGif = false;
        }
    }
}
