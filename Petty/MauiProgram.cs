using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Petty.Extensions;
using Petty.Helpres;
using Petty.Services.Platforms.Audio;
using Petty.Services.Local.PermissionsFolder;
using Petty.ViewModels.Components;
using Petty.ViewModels.Components.GraphicsViews;
using Petty.Views;
using Sharpnado.Tabs;
using SkiaSharp.Views.Maui.Handlers;
using Petty.Services.Platforms.Speech;
using Petty.Services.Platforms.PettyCommands;

namespace Petty
{
    public static class MauiProgram
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSharpnadoTabs(loggerEnable: false)
                .ConfigureMauiHandlers(handlers => { handlers.AddHandler<YinYangSpinnerWithTextSkiaSharpViewModel, SKCanvasViewHandler>(); })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("MonotypeCorsiva.ttf", "MonotypeCorsiva");
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

            var app = builder.Build();
            ServiceProvider = app.Services;
            return app;
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
            builder.Services.AddSingleton<PermissionService>();
            builder.Services.AddSingleton<AudioPlayerService>();
            builder.Services.AddSingleton<WebRequestsService>();
            builder.Services.AddSingleton<LocalizationService>();
            builder.Services.AddSingleton<UserMessagesService>();
            builder.Services.AddSingleton<PettyCommandsService>();
            builder.Services.AddSingleton<AudioRecorderService>();
            builder.Services.AddSingleton<SpeechRecognizerService>();
            builder.Services.AddSingleton<IAudioStream, AudioStream>();
            builder.Services.AddSingleton<IMessenger, WeakReferenceMessenger>();

            builder.Services.AddTransient<WaveRecorderService>();

            return builder;
        }

        private static MauiAppBuilder RegisterPagesWithViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddSingletonWithShellRoute<MainPage, MainViewModel>(RoutesHelper.MAIN);
            builder.Services.AddSingletonWithShellRoute<SettingsPage, SettingsViewModel>(RoutesHelper.SETINGS);
            builder.Services.AddSingletonWithShellRoute<SpeechSimulatorPage, SpeechSimulatorViewModel>(RoutesHelper.SPEECH_SIMULATOR);
            builder.Services.AddSingletonWithShellRoute<DiagnosticPettyPage, DiagnosticPettyViewModel>(
                $"{RoutesHelper.SETINGS}/{RoutesHelper.DIAGNOSTICS}");
            return builder;
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<RunningTextViewModel>();
            return builder;
        }

        private static MauiAppBuilder RegisterModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<Settings>(services => services.GetService<SettingsService>().Settings.CloneJson());
            return builder;
        }
    }
}
