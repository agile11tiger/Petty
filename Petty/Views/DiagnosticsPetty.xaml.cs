using Petty.Resources.Localization;

namespace Petty.Views;

public partial class DiagnosticPettyPage : ContentPage
{
    public DiagnosticPettyPage(AppShellViewModel appShellViewModel, DiagnosticPettyViewModel diagnosticPettyViewModel)
    {
        BindingContext = diagnosticPettyViewModel;
        _appShellViewModel = appShellViewModel;
        InitializeComponent();
    }

    private readonly AppShellViewModel _appShellViewModel;

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _appShellViewModel.Title = AppResources.PageDiagnostics;
    }
}