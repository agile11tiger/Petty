using Mopups.Pages;
namespace Petty.ViewModels.DisplayAlert;

public partial class DisplayAlertPage : PopupPage
{
    public DisplayAlertPage(DisplayAlertViewModel displayAlertViewModel)
    {
        BindingContext = DisplayAlertViewModel = displayAlertViewModel;
        InitializeComponent();
        Background = new Color(0, 0, 0, 0.59f);
    }

    public DisplayAlertViewModel DisplayAlertViewModel { get; }

    protected override void OnDisappearing()
    {
        DisplayAlertViewModel.HandleDisappearingCommand.Execute(null);
        base.OnDisappearing();
    }
}