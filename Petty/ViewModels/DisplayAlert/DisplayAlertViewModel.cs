using Mopups.Pages;
using Petty.Services.Local.UserMessages;
using Petty.ViewModels.Base;
using Petty.Views.Controls;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace Petty.ViewModels.DisplayAlert;

public partial class DisplayAlertViewModel : ViewModelBase
{
    public DisplayAlertViewModel(
        IList links,
        string title = null,
        string cancel = null,
        string accept = null,
        UserMessagesService userMessagesService = null,
        SelectionMode selectionMode = default)
    {
        if (links == null || links.Count == 0)
            throw new ArgumentException("Links can`t be null or empty");

        if (links[0] is not ILink)
            throw new ArgumentException("The list element must implement ILink");

        if (selectionMode == SelectionMode.Multiple)
            throw new NotImplementedException();

        _links = links;
        _titleLabel = title;
        _cancelButton = cancel;
        _acceptButton = accept;
        _selectionMode = selectionMode;
        _userMessagesService = userMessagesService;
        _isVisibleTitleLabel = !string.IsNullOrEmpty(title);
        _isVisibleAcceptButton = !string.IsNullOrEmpty(accept);
        _isVisibleCancelButton = !string.IsNullOrEmpty(cancel);
        _isVisibleFooter = _isVisibleCancelButton || _isVisibleAcceptButton;

        if (AcceptButton != null)
            _waiter = new AutoResetEvent(false);
    }

    private bool _isAccepted;
    private readonly AutoResetEvent _waiter;
    private readonly UserMessagesService _userMessagesService;
    [ObservableProperty] private IList _links;
    [ObservableProperty] private string _titleLabel;
    [ObservableProperty] private ILink _selectedLink;
    [ObservableProperty] private string _acceptButton;
    [ObservableProperty] private string _cancelButton;
    [ObservableProperty] private bool _isVisibleFooter;
    [ObservableProperty] private bool _isVisibleTitleLabel;
    [ObservableProperty] private bool _isVisibleAcceptButton;
    [ObservableProperty] private bool _isVisibleCancelButton;
    [ObservableProperty] private SelectionMode _selectionMode;
    public ICommand ClosingCommand { get; set; }

    public async Task<bool> GetResultAsync(PopupPage displayAlertPage)
    {
        if (AcceptButton != null)
            await Task.Run(_waiter.WaitOne);

        return _isAccepted;
    }

    [RelayCommand]
    private async Task SelectionChangedAsync(PopupPage displayAlertPage)
    {
        await CloseAsync(displayAlertPage);
    }
        
    [RelayCommand]
    private async Task LinkTappedAsync(Func<Task> func)
    {
        await func();
    }
        
    [RelayCommand]
    private async Task AcceptAsync(PopupPage displayAlertPage)
    {
        _isAccepted = true;
        await CloseAsync(displayAlertPage);
    }

    [RelayCommand]
    private async Task CancelAsync(PopupPage displayAlertPage)
    {
        await CloseAsync(displayAlertPage);
    }

    [RelayCommand]
    private void HandleDisappearing()
    {
        ClosingCommand?.Execute(SelectedLink);

        if (AcceptButton != null)
            _waiter.Set();
    }

    private async Task CloseAsync(PopupPage displayAlertPage)
    {
        await _userMessagesService.RemovePageAsync(displayAlertPage);
    }
}