namespace Petty.Views;

public partial class DiagnosticPettyPage : ContentPage
{
	public DiagnosticPettyPage(DiagnosticPettyViewModel pettyTestViewModel)
	{
        BindingContext = pettyTestViewModel;
		InitializeComponent();
	}
}