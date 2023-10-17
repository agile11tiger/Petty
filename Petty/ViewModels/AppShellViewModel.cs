using CommunityToolkit.Mvvm.Messaging;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;
using Petty.ViewModels.Base;
using Petty.ViewModels.Components;

namespace Petty.ViewModels
{
    public partial class AppShellViewModel : ViewModelBase
    {
        public AppShellViewModel(IMessenger messenger, RunningTextViewModel runningTextViewModel)
        {
            _messenger = messenger;
            _runningTextViewModel = runningTextViewModel;
            messenger.Register<UpdateProgressBar>(this, (recipient, message) => UpdateProgressSomeBackgroundWorking(message.Percentages));
        }

        private readonly IMessenger _messenger;
        [ObservableProperty] private string _title;
        [ObservableProperty] private bool _isRunningProgressBar;
        [ObservableProperty] private bool _isVisibleQuestionIcon;
        [ObservableProperty] private double _progressBarPercentages;
        [ObservableProperty] private bool _isAnimationPlayingCoffeeGif;
        [ObservableProperty] private RunningTextViewModel _runningTextViewModel;

        public IAsyncRelayCommand ShowQuestionIconInfo;
        public Action InvalidateProgressBar { get; set; }

        [RelayCommand]
        private async Task StartCoffeeGifAsync()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(3000);
                IsAnimationPlayingCoffeeGif = true;
            });
        }

        [RelayCommand]
        private void StopCoffeeGif()
        {
            IsAnimationPlayingCoffeeGif = false;
        }

        [RelayCommand]
        private async Task TapQuestionIconAsync()
        {
            if (ShowQuestionIconInfo != null)
                await ShowQuestionIconInfo.ExecuteAsync(null);
        }

        private void UpdateProgressSomeBackgroundWorking(double percentages)
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
