using Petty.ViewModels.Base;

namespace Petty.ViewModels.Components.DisplayAlert
{
    public partial class DisplayAlertViewModel : ViewModelBase
    {
        public DisplayAlertViewModel(IList<ILink> links, string title = null, string cancel = null, string accept = null)
        {
            Links = links;
            _titleLabel = title;
            _acceptButton = accept;
            _cancelButton = cancel;
            _isVisibleTitleLabel = !string.IsNullOrEmpty(title);
            _isVisibleAcceptButton = !string.IsNullOrEmpty(accept);
            _isVisibleCancelButton = !string.IsNullOrEmpty(cancel);
            _isVisibleFooter = _isVisibleCancelButton || _isVisibleAcceptButton;
        }

        [ObservableProperty] private string _titleLabel;
        [ObservableProperty] private string _acceptButton;
        [ObservableProperty] private string _cancelButton;
        [ObservableProperty] private bool _isVisibleFooter;
        [ObservableProperty] private bool _isVisibleTitleLabel;
        [ObservableProperty] private bool _isVisibleAcceptButton;
        [ObservableProperty] private bool _isVisibleCancelButton;
        [ObservableProperty] private IList<ILink> _links;
    }
}
