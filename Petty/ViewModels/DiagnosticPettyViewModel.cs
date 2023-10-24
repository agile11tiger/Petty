using Petty.Resources.Localization;
namespace Petty.ViewModels;

public partial class DiagnosticPettyViewModel() : ViewModelBase
{
    [RelayCommand]
    private void NavigatedTo(NavigatedToEventArgs args)
    {
        _appShellViewModel.Title = AppResources.PageDiagnostics;
    }
}
