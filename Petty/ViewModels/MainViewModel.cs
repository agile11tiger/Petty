using CommunityToolkit.Mvvm.Messaging;
using Petty.Helpres;
using Petty.Services.Local.UserMessages;
using Petty.ViewModels.Base;

namespace Petty.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public MainViewModel(IMessenger messenger, UserMessagesService userMessagesService)
        {
            _messenger = messenger;
            _userMessagesService = userMessagesService;
        }

        private readonly IMessenger _messenger;
        private readonly UserMessagesService _userMessagesService;
        [ObservableProperty] private bool _isSelectedTabBarItem;
        [ObservableProperty] private string _pettyGuardIconImageSource = "play.png";

        [RelayCommand]
        private async Task GoToSettingsAsync()
        {
            await _navigationService.GoToAsync(RoutesHelper.SETTINGS);
            IsSelectedTabBarItem = false;
        }

        [RelayCommand]
        private async Task GoToSpeechSimulator()
        {
            await _navigationService.GoToAsync(RoutesHelper.SPEECH_SIMULATOR);
        }

        [RelayCommand]
        private async Task GoToLeaderboardAsync()
        {
        }
    }
}
