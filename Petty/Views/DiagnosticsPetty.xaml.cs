namespace Petty.Views;

public partial class DiagnosticPettyPage : ContentPage
{
    public DiagnosticPettyPage(DiagnosticPettyViewModel diagnosticPettyViewModel)
    {
        BindingContext = diagnosticPettyViewModel;
        Behaviors.Add(new EventToCommandBehavior { EventName = nameof(NavigatedTo), Command = diagnosticPettyViewModel.NavigatedToCommand });
        InitializeComponent();
    }
}