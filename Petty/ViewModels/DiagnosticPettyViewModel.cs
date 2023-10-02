using CommunityToolkit.Mvvm.Messaging;
using Petty.ViewModels.Base;
using Petty.Resources.Localization;
using Petty.MessengerCommands.FromPettyGuard;
using Petty.Services.Platforms.Speech;
using System.Text;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;

namespace Petty.ViewModels
{
    public partial class DiagnosticPettyViewModel : ViewModelBase
    {
        public DiagnosticPettyViewModel(
            LoggerService loggerService, 
            NavigationService navigationService, 
            LocalizationService localizationService) 
            : base(loggerService, navigationService, localizationService)
        {
        }
    }
}
