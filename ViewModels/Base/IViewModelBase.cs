using Petty.Services.Navigation;

namespace Petty.ViewModels.Base
{
    internal interface IViewModelBase
    {
        public INavigationService NavigationService { get; }
        public IAsyncRelayCommand InitializeAsyncCommand { get; }
        public bool IsBusy { get; }
        public bool IsInitialized { get; }
        Task InitializeAsync();
    }
}
