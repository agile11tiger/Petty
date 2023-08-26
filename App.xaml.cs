using Petty.Services.Navigation;

namespace Petty
{
    public partial class App : Application
    {
        public App(AppShellViewModel appShellViewModel, INavigationService navigationService)
        {
            //TODO: По желанию поддержать две темы и в настройках давать выбор.
            UserAppTheme = AppTheme.Light;
            InitializeComponent();
            MainPage = new AppShell(appShellViewModel, navigationService);
        }

    }
}
