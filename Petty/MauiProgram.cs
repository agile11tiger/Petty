using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Petty.Extensions;
using Petty.Helpres;
using Petty.Services.Local.Audio;
using Petty.Services.Local.Localization;
using Petty.Services.Local.PettyCommands;
using Petty.ViewModels.Components;
using Petty.Views;
using Sharpnado.Tabs;
using System.Globalization;
using System.Threading;
using System.Xml.Xsl;

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
                .RegisterViewModels()
                .RegisterModels();

            builder.Services.AddDbContext<ApplicationDbContext>();
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
            builder.Services.AddSingleton<LoggerService>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<PettyVoiceService>();
            builder.Services.AddSingleton<NavigationService>();
            builder.Services.AddSingleton<LocalizationService>();
            builder.Services.AddSingleton<PettyCommandsService>();
            return builder;
        }

        private static MauiAppBuilder RegisterPagesWithViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransientWithShellRoute<MainPage, MainViewModel>(RoutesHelper.MAIN);
            builder.Services.AddTransientWithShellRoute<SettingsPage, SettingsViewModel>(RoutesHelper.SETINGS);
            return builder;
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<AppShellViewModel>();
            builder.Services.AddTransient<RunningTextViewModel>();
            return builder;
        }

        private static MauiAppBuilder RegisterModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<Settings>(services => services.GetService<SettingsService>().Settings.CloneJson());
            return builder;
        }
    }
}
