using Petty.Resources.Localization;

namespace Petty.Views;

public partial class DiagnosticPettyPage : ContentPage
{
	public DiagnosticPettyPage(DiagnosticPettyViewModel diagnosticPettyViewModel, AppShellViewModel appShellViewModel)
	{
        BindingContext = diagnosticPettyViewModel;
        _appShellViewModel = appShellViewModel;
        _diagnosticPettyViewModel = diagnosticPettyViewModel;
        InitializeComponent();
    }

    private AppShell _appShell;
    private readonly AppShellViewModel _appShellViewModel;
    private readonly DiagnosticPettyViewModel _diagnosticPettyViewModel;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(10); //Otherwise, the question mark is shown before moving to a new page.
        _appShellViewModel.Title = AppResources.DiagnosticsPetty;
        _appShellViewModel.IsVisibleQuestionIcon = true;
        _appShellViewModel.ShowQuestionIconInfo = _diagnosticPettyViewModel.ShowQuestionIconInfoCommand;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _appShellViewModel.IsVisibleQuestionIcon = false;
        _appShellViewModel.ShowQuestionIconInfo = null;
    }
}