using CommunityToolkit.Mvvm.Messaging;
using Petty.Helpres;
using Petty.ViewModels.Base;

namespace Petty.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public MainViewModel(
            IMessenger messenger,
            LoggerService loggerService,
            NavigationService navigationService,
            UserMessagesService userMessagesService,
            LocalizationService localizationService)
            : base(loggerService, navigationService, localizationService)
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
            await NavigationService.GoToAsync(RoutesHelper.SETINGS);
            IsSelectedTabBarItem = false;
        }

        [RelayCommand]
        private async Task GoToSpeechSimulator()
        {
            await NavigationService.GoToAsync(RoutesHelper.SPEECH_SIMULATOR);
        }

        [RelayCommand]
        private async Task GoToLeaderboardAsync()
        {
        }
    }
}
