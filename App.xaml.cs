using Petty.Services.Navigation;

namespace Petty
{
    public partial class App : Application
    {
        public App(INavigationService navigationService)
        {
            _navigationService = navigationService;
            //TODO: По желанию поддержать две темы и в настройках давать выбор.
            UserAppTheme = AppTheme.Light;
            InitializeComponent();
            MainPage = new AppShell(_navigationService);
        }

        private readonly INavigationService _navigationService;
    }
}
