using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Petty.Services.Navigation;
using Petty.Services.Settings;
using Petty.Views;
using Sharpnado.Tabs;

namespace Petty
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSharpnadoTabs(loggerEnable: false)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Bold.ttf", "OpenSansBold");
                    fonts.AddFont("OpenSans-ExtraBold.ttf", "OpenSansExtraBold");
                    fonts.AddFont("OpenSans-Light.ttf", "OpenSansLight");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .RegisterAppServices()
                .RegisterPagesWithViewModels()
                .RegisterViewModels();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            TaskScheduler.UnobservedTaskException += HandleUnobservedException;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleCurrentDomainUnhandledException);
            return builder.Build();
        }

        private static void HandleUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //TODO: need logging
            throw new NotImplementedException();
        }

        private static void HandleCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //TODO: need logging
            throw new NotImplementedException();
        }

        private static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            return builder;
        }

        private static MauiAppBuilder RegisterPagesWithViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransientWithShellRoute<MainPage, MainViewModel>("Main");
            builder.Services.AddTransientWithShellRoute<SettingsPage, SettingsViewModel>("Settings");
            return builder;
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            return builder;
        }
    }
}
