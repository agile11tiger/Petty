using CommunityToolkit.Mvvm.Messaging;
using Petty.Helpers;
using Petty.Resources.Localization;
using Petty.Services.Local.PermissionsFolder;
namespace Petty.ViewModels;

public partial class MainViewModel(IMessenger _messenger, PermissionService _permissionService) : ViewModelBase
{
    [ObservableProperty] private bool _isSelectedTabBarItem;
    [ObservableProperty] private string _pettyGuardIconImageSource = "play.png";

    [RelayCommand]
    private async Task OnNavigatedTo(NavigatedToEventArgs args)
    {
        _appShellViewModel.Title = AppResources.PagePetty;
        await _permissionService.GetAllPermissionsAsync();
    }

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
