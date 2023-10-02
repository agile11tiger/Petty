using Petty.Resources.Localization;

namespace Petty.Views;

public partial class DiagnosticPettyPage : ContentPage
{
	public DiagnosticPettyPage(DiagnosticPettyViewModel diagnosticPettyViewModel)
	{
        BindingContext = _diagnosticPettyViewModel = diagnosticPettyViewModel;
        InitializeComponent();
    }

    private readonly DiagnosticPettyViewModel _diagnosticPettyViewModel;

}