using CommunityToolkit.Mvvm.Messaging;
using Petty.ViewModels.Base;
using Petty.ViewModels.Components;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;

namespace Petty.ViewModels
{
    public partial class AppShellViewModel : ViewModelBase
    {
        public AppShellViewModel(
            IMessenger messenger,
            LoggerService loggerService,
            NavigationService navigationService,
            LocalizationService localizationService,
            RunningTextViewModel runningTextViewModel)
            : base(loggerService, navigationService, localizationService)
        {
            _messenger = messenger;
            _runningTextViewModel = runningTextViewModel;
            messenger.Register<UpdateProgressBar>(this, (recipient, message) => UpdateProgressSomeBackgroundWorking(message.Percentages));
        }

        private readonly IMessenger _messenger;
        [ObservableProperty] private bool _isRunningProgressBar;
        [ObservableProperty] private double _progressBarPercentages;
        [ObservableProperty] private bool _isAnimationPlayingCoffeeGif;
        [ObservableProperty] private RunningTextViewModel _runningTextViewModel;
        public Action InvalidateProgressBar { get; set; }

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

        public void UpdateProgressSomeBackgroundWorking(double percentages)
        {
            App.Current.Dispatcher.Dispatch(() =>
            {
                IsRunningProgressBar = true;
                ProgressBarPercentages = percentages;

                if (percentages >= 99)
                    IsRunningProgressBar = false;

                InvalidateProgressBar?.Invoke();
            });
        }
    }
}
