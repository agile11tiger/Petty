using Petty.Services.Navigation;
using Petty.Views;
using static System.Net.Mime.MediaTypeNames;

namespace Petty
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel appShellViewModel, INavigationService navigationService)
        {
            BindingContext = _appShellViewModel = appShellViewModel;
            _navigationService = navigationService;
            InitializeComponent();
        }

        private readonly AppShellViewModel _appShellViewModel;
        private readonly INavigationService _navigationService;

        protected override async void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler is not null)
            {
                await _navigationService.InitializeAsync();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _appShellViewModel.StartRunningTextCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _appShellViewModel.StartRunningTextCommand.Execute(null);
        }
    }
}
