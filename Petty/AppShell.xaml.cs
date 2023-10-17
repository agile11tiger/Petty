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
            appShellViewModel.InvalidateProgressBar = _circularProgressBarGraphicsView.Invalidate;
        }

        private readonly AppShellViewModel _appShellViewModel;
        private readonly NavigationService _navigationService;

        protected override async void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler is not null)
            {
                //todo:change
                //await _navigationService.InitializeAsync();
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _appShellViewModel.RunningTextViewModel.StartRunningTextCommand.Execute(null);
            await _appShellViewModel.StartCoffeeGifCommand.ExecuteAsync(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _appShellViewModel.RunningTextViewModel.StopRunningTextCommand.Execute(null);
            _appShellViewModel.StopCoffeeGifCommand.Execute(null);
        }
    }
}
