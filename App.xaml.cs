using CommunityToolkit.Mvvm.Messaging;
using Petty.WeakReferenceMessengerCommands;
using System.Globalization;

namespace Petty
{
    public partial class App : Application
    {
        public App(
            AppShellViewModel appShellViewModel,
            NavigationService navigationService)
        {
            //TODO: По желанию поддержать две темы и в настройках давать выбор.
            //https://learn.microsoft.com/en-us/dotnet/maui/user-interface/theming
            //https://www.youtube.com/watch?v=0cY8iCz50fI&ab_channel=DanielHindrikes
            //https://www.youtube.com/watch?v=eu52qX-qww4&ab_channel=ProgrammingWithChris
            UserAppTheme = AppTheme.Light;
            WeakReferenceMessenger.Default.Register<RestartCommand>(this, (sender, message) =>
            {
                MainPage.Dispatcher.Dispatch(() =>
                {
                    LocalizationService.SetCulture(message.CultureInfo);
                    MainPage = new AppShell(appShellViewModel, navigationService);//REQUIRE RUN MAIN THREAD
                });
            });
            InitializeComponent();
            MainPage = new AppShell(appShellViewModel, navigationService);
        }
    }
}
