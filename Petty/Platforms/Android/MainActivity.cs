using Android.App;
using Android.Content.PM;
using CommunityToolkit.Mvvm.Messaging;
using Petty.MessengerCommands.ToPettyGuard;

namespace Petty
{
    //[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [Activity(Theme = "@style/Maui.MainTheme.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public MainActivity()
        {
            var messenger = IPlatformApplication.Current.Services.GetService<IMessenger>();
            messenger.Register<StartPettyGuardService>(this, (recipient, message) => PettyGuardAndroidService.Start());
            messenger.Register<StopPettyGuardService>(this, (recipient, message) => PettyGuardAndroidService.Stop());
        }
    }
}
