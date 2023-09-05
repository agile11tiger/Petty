using CommunityToolkit.Mvvm.Messaging;
using Petty.Helpres;
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
            Initilize();
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

        private static void Initilize()
        {
            var isFirstRun = Preferences.Default.Get<bool>(PreferencesHelper.IS_FIRST_RUN, true);
            var language = Preferences.Default.Get<string>(PreferencesHelper.LANGUAGE, null);

            if (isFirstRun is true)
            {
                Preferences.Default.Set<bool>(PreferencesHelper.IS_FIRST_RUN, false);
            }

            if (language is not null)
                LocalizationService.SetCulture(new CultureInfo(language));
        }

        public static void StartPettyGuadrAndroidService()
        {
#if ANDROID
            var intent = new Android.Content.Intent(Android.App.Application.Context, typeof(PettyGuardService));
            //TODO: если отзовут разрешение надо потребовать чтобы влючили
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                Android.App.Application.Context.StartForegroundService(intent);
            else
                Android.App.Application.Context.StartService(intent);
#endif
        }
    }
}
