using CommunityToolkit.Mvvm.Messaging;
using Petty.Helpres;
using Petty.MessengerCommands.Application;
using System.Globalization;

namespace Petty
{
    public partial class App : Application
    {
        public App(
            IMessenger messenger,
            AppShellViewModel appShellViewModel,
            NavigationService navigationService,
            LocalizationService localizationService)
        {
            Initilize(localizationService);
            //TODO: По желанию поддержать две темы и в настройках давать выбор.
            //https://learn.microsoft.com/en-us/dotnet/maui/user-interface/theming
            //https://www.youtube.com/watch?v=0cY8iCz50fI&ab_channel=DanielHindrikes
            //https://www.youtube.com/watch?v=eu52qX-qww4&ab_channel=ProgrammingWithChris
            UserAppTheme = AppTheme.Light;
            messenger.Register<RestartApplication>(this, (recipient, message) =>
            {
                MainPage.Dispatcher.Dispatch(() =>
                {
                    localizationService.SetCulture(message.CultureInfo);
                    MainPage = new AppShell(appShellViewModel, navigationService);//REQUIRE RUN MAIN THREAD
                });
            });
            InitializeComponent();


            MainPage = new AppShell(appShellViewModel, navigationService); //new SplashScreenPage();
            //_ = EndSplash(appShellViewModel, navigationService);
        }
        async Task EndSplash(AppShellViewModel appShellViewModel,
            NavigationService navigationService)
        {
            //delay 3 seconds
            await Task.Delay(5000);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                MainPage = new AppShell(appShellViewModel, navigationService);
            });
        }

        private void Initilize(LocalizationService localizationService)
        {
            var isFirstRun = Preferences.Default.Get<bool>(SharedPreferencesHelper.IS_FIRST_RUN, true);
            var language = Preferences.Default.Get<string>(SharedPreferencesHelper.LANGUAGE, null);

            if (isFirstRun is true)
            {
                Preferences.Default.Set<bool>(SharedPreferencesHelper.IS_FIRST_RUN, false);
            }

            if (language is not null)
                localizationService.SetCulture(new CultureInfo(language));
        }
    }
}
