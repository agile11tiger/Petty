using Petty.Services.Navigation;
using Petty.ViewModels.Components;

namespace Petty
{
    public partial class App : Application
    {
        public App(
            AppShellViewModel appShellViewModel,
            INavigationService navigationService)
        {
            //TODO: По желанию поддержать две темы и в настройках давать выбор.
            //https://learn.microsoft.com/en-us/dotnet/maui/user-interface/theming
            //https://www.youtube.com/watch?v=0cY8iCz50fI&ab_channel=DanielHindrikes
            //https://www.youtube.com/watch?v=eu52qX-qww4&ab_channel=ProgrammingWithChris
            UserAppTheme = AppTheme.Light;
            InitializeComponent();
            MainPage = new AppShell(appShellViewModel, navigationService);
        }

    }
}
