namespace Petty.ViewModels.Base
{
    internal interface IViewModelBase
    {
        public NavigationService NavigationService { get; }
        public IAsyncRelayCommand InitializeAsyncCommand { get; }
        public bool IsBusy { get; }
        public bool IsInitialized { get; }
        Task InitializeAsync();
    }
}
