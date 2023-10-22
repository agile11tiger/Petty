using CommunityToolkit.Mvvm.Messaging;
using Petty.Helpers;
using Petty.MessengerCommands.Application;
using Petty.ViewModels.Base;
using System.Globalization;
namespace Petty.ViewModels;

public partial class MainViewModel(IMessenger _messenger) : ViewModelBase
{
    [ObservableProperty] private bool _isSelectedTabBarItem;
    [ObservableProperty] private string _pettyGuardIconImageSource = "play.png";

    [RelayCommand]
    private async Task GoToSettingsAsync()
    {
        await _navigationService.GoToAsync(RoutesHelper.SETTINGS);
        IsSelectedTabBarItem = false;
    }

    [RelayCommand]
    private async Task GoToSpeechSimulatorAsync()
    {
        await _navigationService.GoToAsync(RoutesHelper.SPEECH_SIMULATOR);
    }

    [RelayCommand]
    private async Task GoToLeaderboardAsync()
    {
    }
}
