using Mopups.Pages;
namespace Petty.ViewModels.DisplayAlert;

public partial class DisplayAlertPage : PopupPage
{
    public DisplayAlertPage(DisplayAlertViewModel displayAlertViewModel)
    {
        BindingContext = DisplayAlertViewModel = displayAlertViewModel;
        Behaviors.Add(new EventToCommandBehavior { EventName = nameof(Disappearing), Command = displayAlertViewModel.DisappearingCommand });
        InitializeComponent();
        Background = new Color(0, 0, 0, 0.59f);
    }

    public DisplayAlertViewModel DisplayAlertViewModel { get; }
}