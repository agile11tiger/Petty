using Mopups.Pages;
using Petty.ViewModels.Components.DisplayAlert;

namespace Petty.Views.Components;

public partial class DisplayAlertPage : PopupPage
{
    public DisplayAlertPage(DisplayAlertViewModel displayAlertViewModel)
    {
        BindingContext = _displayAlertViewModel = displayAlertViewModel;
        InitializeComponent();
        _diplayAlertPage.BackgroundColor = new Color(0, 0, 0, 0.59f);
    }

    public readonly DisplayAlertViewModel _displayAlertViewModel;

    private async void Link_TappedAsync(object sender, TappedEventArgs e)
    {
        await (e.Parameter as Func<Task>)();
    }

    private void LinkAcceptTapped(object sender, TappedEventArgs e)
    {
        //todo implement 
        base.OnBackButtonPressed();
    }

    private void LinkCancelTapped(object sender, TappedEventArgs e)
    {
        base.OnBackButtonPressed();
    }
}