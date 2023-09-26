using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;
using Petty.Services.Local.Localization;
using Petty.Services.Local;
using Petty.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Petty.MessengerCommands.ToPettyGuard;
using Petty.Resources.Localization;

namespace Petty.ViewModels
{
    public partial class DiagnosticPettyViewModel : ViewModelBase
    {
        public DiagnosticPettyViewModel(
            IMessenger messenger,
            LoggerService loggerService,
            NavigationService navigationService,
            UserMessagesService userMessagesService,
            LocalizationService localizationService)
            : base(loggerService, navigationService, localizationService)
        {
            _messenger = messenger;
            _userMessagesService = userMessagesService;
            _messenger.Register<StartedPettyGuardService>(this, (recipient, message) => IsStartingPettyGuardAndroidService = message.IsStarted);
            _messenger.Register<StoppedPettyGuardService>(this, (recipient, message) => IsStartingPettyGuardAndroidService = !message.IsStopped);
            _messenger.Register<SendSpeech>(this, (recipient, message) => Speech = message.Speech);
        }

        private readonly IMessenger _messenger;
        private readonly UserMessagesService _userMessagesService;
        [ObservableProperty] private string _speech = "lol";
        [ObservableProperty] private bool _isStartingPettyGuardAndroidService;

        [RelayCommand]
        private async Task StartStopPettyGuardAndroidService()
        {
            if (!IsStartingPettyGuardAndroidService)
                _messenger.Send<StartPettyGuardService>();
            else if (await _userMessagesService.SendRequestAsync(AppResources.DisablePettyGuard, AppResources.No, AppResources.Yes))
                _messenger.Send<StopPettyGuardService>();
        }
    }
}
