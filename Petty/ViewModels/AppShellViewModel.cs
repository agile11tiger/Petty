using CommunityToolkit.Mvvm.Messaging;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;
using Petty.ViewModels.Base;
using Petty.Views.Controls;
namespace Petty.ViewModels;

public partial class AppShellViewModel : ViewModelBase
{
    public AppShellViewModel(IMessenger messenger, RunningTextViewModel runningTextViewModel)
    {
        _runningTextViewModel = runningTextViewModel;
        messenger.Register<UpdateProgressBar>(this, (recipient, message) => UpdateProgressSomeBackgroundWorking(message.Percentages));
    }

    private bool _isFlyoutOpen;
    [ObservableProperty] private string _title;
    [ObservableProperty] private bool _isRunningProgressBar;
    [ObservableProperty] private bool _isVisibleQuestionIcon;
    [ObservableProperty] private double _progressBarPercentages;
    [ObservableProperty] private bool _isAnimationPlayingCoffeeGif;
    [ObservableProperty] private RunningTextViewModel _runningTextViewModel;

    public IAsyncRelayCommand ShowQuestionIconInfo;
    public Action InvalidateProgressBar { get; set; }
    public bool IsFlyoutOpen
    {
        get => _isFlyoutOpen;
        set 
        {
            if (!_isFlyoutOpen) //opening now
            {
                RunningTextViewModel.StartRunningTextCommand.Execute(null);
                IsAnimationPlayingCoffeeGif = true;
            }
            else
            {
                RunningTextViewModel.StopRunningTextCommand.Execute(null);
                IsAnimationPlayingCoffeeGif = false;
            }

            SetProperty(ref _isFlyoutOpen, value);
        } 
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
