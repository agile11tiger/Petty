using Petty.ViewModels.Base;

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
