using CommunityToolkit.Mvvm.Messaging;
using Petty.Helpres;
using Petty.Resources.Localization;
using Petty.ViewModels.Base;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;
using Petty.MessengerCommands.ToPettyGuard;

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
            messenger.Register<StartedPettyGuardService>(this, (recipient, message) => IsStartingPettyGuardAndroidService = message.IsStarted);
            messenger.Register<StoppedPettyGuardService>(this, (recipient, message) => IsStartingPettyGuardAndroidService = !message.IsStopped);
            messenger.Register<SendSpeech>(this, (recipient, message) => Speech = message.Speech);
        }

        private readonly IMessenger _messenger;
        private readonly UserMessagesService _userMessagesService;
        [ObservableProperty] private string _speech = "lol";
        [ObservableProperty] private bool _isSelectedTabBarItem;
        [ObservableProperty] private bool _isStartingPettyGuardAndroidService;
        [ObservableProperty] private string _pettyGuardIconImageSource = "play.png";

        [RelayCommand]
        private async Task GoToSettingsAsync()
        {
            await NavigationService.GoToAsync(RoutesHelper.SETINGS);
            IsSelectedTabBarItem = false;
        }

        [RelayCommand]
        private async Task StartStopPettyGuardAndroidService()
        {
            if (!IsStartingPettyGuardAndroidService)
                _messenger.Send<StartPettyGuardService>();
            else if (await _userMessagesService.SendRequestAsync(AppResources.DisablePettyGuard, AppResources.No, AppResources.Yes))
                _messenger.Send<StopPettyGuardService>();
        }

        [RelayCommand]
        private async Task GoToLeaderboardAsync()
        {
        }
    }
}
