using Petty.Services.Navigation;

namespace Petty
{
    public partial class App : Application
    {
        public App(INavigationService navigationService)
        {
            _navigationService = navigationService;

            InitializeComponent();
            MainPage = new AppShell(_navigationService);
        }

        private readonly INavigationService _navigationService;
    }
}
