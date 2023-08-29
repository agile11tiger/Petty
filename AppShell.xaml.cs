using Petty.ViewModels.Components;

namespace Petty
{
    public partial class AppShell : Shell
    {
        public AppShell(
            AppShellViewModel appShellViewModel,
            NavigationService navigationService)
        {
            BindingContext = _appShellViewModel = appShellViewModel;
            _navigationService = navigationService;
            InitializeComponent();
        }

        private readonly AppShellViewModel _appShellViewModel;
        public readonly RunningTextViewModel RunningTextViewModel;
        private readonly NavigationService _navigationService;

        protected override async void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler is not null)
            {
                await _navigationService.InitializeAsync();
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _appShellViewModel.RunningTextViewModel.StartRunningTextCommand.Execute(null);
            await _appShellViewModel.StartCoffeGifCommand.ExecuteAsync(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _appShellViewModel.RunningTextViewModel.StopRunningTextCommand.Execute(null);
            _appShellViewModel.StopCoffeGifCommand.Execute(null);
        }
    }
}
