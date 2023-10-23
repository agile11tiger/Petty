using CommunityToolkit.Mvvm.Messaging;
using Petty.Helpers;
using Petty.MessengerCommands.Application;
using System.Globalization;
namespace Petty;

public partial class App : Application
{
    public App(
        IMessenger messenger,
        IVersionTracking versionTracking,
        AppShellViewModel appShellViewModel,
        NavigationService navigationService,
        LocalizationService localizationService)
    {
        Initialize(localizationService, versionTracking);
        //TODO: По желанию поддержать две темы и в настройках давать выбор.
        //https://learn.microsoft.com/en-us/dotnet/maui/user-interface/theming
        //https://www.youtube.com/watch?v=0cY8iCz50fI&ab_channel=DanielHindrikes
        //https://www.youtube.com/watch?v=eu52qX-qww4&ab_channel=ProgrammingWithChris
        UserAppTheme = AppTheme.Light;
        messenger.Register<RestartApplication>(this, (recipient, message) =>
        {
            MainPage.Dispatcher.Dispatch(() =>
            {
                //Application.Current.Quit(); not good
                //todo: on net8 not working
                //MainPage = new AppShell(appShellViewModel);//REQUIRE RUN MAIN THREAD
                //not working https://github.com/Metlina/XamarinRestartAndroidAppOnCrash/blob/master/RestartAndroidForms/RestartAndroidForms.Android/MyApplication.cs
            });
        });
        InitializeComponent();


        MainPage = new AppShell(appShellViewModel); //new SplashScreenPage();
        //_ = EndSplash(appShellViewModel, navigationService);
    }

    async Task EndSplash(AppShellViewModel appShellViewModel,
        NavigationService navigationService)
    {
        //delay 3 seconds
        await Task.Delay(5000);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            MainPage = new AppShell(appShellViewModel);
        });
    }

    private void Initialize(LocalizationService localizationService, IVersionTracking versionTracking)
    {
        var language = Preferences.Default.Get<string>(SharedPreferencesHelper.LANGUAGE, null);

        if (language is not null)
            localizationService.SetCulture(new CultureInfo(language));
    }
}
