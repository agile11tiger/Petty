using Mopups.Pages;
using Petty.Services.Local.UserMessages;
using Petty.ViewModels.Components.DisplayAlert;

namespace Petty.Views.Components;

public partial class DisplayAlertPage : PopupPage
{
    public DisplayAlertPage(DisplayAlertViewModel displayAlertViewModel, UserMessagesService userMessagesService)
    {
        BindingContext = _displayAlertViewModel = displayAlertViewModel;
        _userMessagesService = userMessagesService;

        if (displayAlertViewModel.AcceptButton != null)
            _waiter = new AutoResetEvent(false);

        InitializeComponent();
        _diplayAlertPage.BackgroundColor = new Color(0, 0, 0, 0.59f);
    }

    private bool _result;
    private readonly AutoResetEvent _waiter;
    private DisplayAlertViewModel _displayAlertViewModel;
    private readonly UserMessagesService _userMessagesService;

    private async void LinkTappedAsync(object sender, TappedEventArgs e)
    {
        await (e.Parameter as Func<Task>)();
    }

    private async void LinkAcceptAsyncTapped(object sender, TappedEventArgs e)
    {
        _result = true;
        await _userMessagesService.RemovePageAsync(this);
    }

    private async void LinkCancelAsyncTapped(object sender, TappedEventArgs e)
    {
        await _userMessagesService.RemovePageAsync(this);
    }

    protected override void OnDisappearing()
    {
        if (_displayAlertViewModel.AcceptButton != null)
            _waiter.Set();

        base.OnDisappearing();
    }

    public async Task<bool> GetResultAsync()
    {
        return _displayAlertViewModel.AcceptButton != null
            && await Task.Run(() =>
            {
                _waiter.WaitOne();
                return _result;
            });
    }
}