namespace Petty.Views;

public partial class SpeechSimulatorPage : ContentPage
{
    public SpeechSimulatorPage(SpeechSimulatorViewModel speechSimulatorViewModel)
    {
        BindingContext = speechSimulatorViewModel;
        Behaviors.Add(new EventToCommandBehavior { EventName = nameof(Appearing), Command = speechSimulatorViewModel.AppearingCommand });
        Behaviors.Add(new EventToCommandBehavior { EventName = nameof(Disappearing), Command = speechSimulatorViewModel.DisappearingCommand });
        InitializeComponent();
    }
}