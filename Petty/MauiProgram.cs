using Android.App;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Petty.Extensions;
using Petty.Helpres;
using Petty.Platforms.Android.Services;
using Petty.Platforms.Android.Services.Audio;
using Petty.Services.Local.PermissionsFolder;
using Petty.Services.Platforms;
using Petty.Services.Platforms.Audio;
using Petty.Services.Platforms.PettyCommands;
using Petty.Services.Platforms.Speech;
using Petty.ViewModels.Components;
using Petty.ViewModels.Components.GraphicsViews;
using Petty.ViewModels.Settings;
using Petty.Views;
using Petty.Views.Settings;
using Plugin.Maui.Audio;
using Sharpnado.Tabs;
using SkiaSharp.Views.Maui.Handlers;

//https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/flashlight?tabs=android#:~:text=%5B-,assembly,-%3A%20UsesFeature(%22android.hardware.camera%22
[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]
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
                //https://stackoverflow.com/questions/72463558/how-to-play-an-audio-file-net-maui
                //.UseMauiCommunityToolkitMediaElement()  https://stackoverflow.com/questions/75525722/correct-way-to-set-net-maui-mediaelement-source-from-code
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
            
            //https://learn.microsoft.com/en-us/dotnet/maui/user-interface/handlers/customize
            AddEditorCustomization();
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
            builder.Services
                .AddSingleton<VoiceService>()
                .AddSingleton<PhoneService>()
                .AddSingleton<LoggerService>()
                .AddSingleton<DatabaseService>()
                .AddSingleton<SettingsService>()
                .AddSingleton<NavigationService>()
                .AddSingleton<PermissionService>()
                .AddSingleton<WebRequestsService>()
                .AddSingleton<AudioPlayerService>()
                .AddSingleton<LocalizationService>()
                .AddSingleton<UserMessagesService>()
                .AddSingleton<PettyCommandsService>()
                .AddSingleton<AudioRecorderService>()
                .AddSingleton<SpeechRecognizerService>()
                .AddSingleton<IAudioStream, AudioStream>()
                .AddSingleton<IMessenger, WeakReferenceMessenger>()
                .AddSingleton<IAudioManager>((services) => AudioManager.Current)

                .AddTransient<WaveRecorderService>();

            return builder;
        }

        private static MauiAppBuilder RegisterPagesWithViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddSingletonWithShellRoute<MainPage, MainViewModel>(RoutesHelper.MAIN)
                .AddSingletonWithShellRoute<SettingsPage, SettingsViewModel>(RoutesHelper.SETTINGS)
                .AddSingletonWithShellRoute<SpeechSimulatorPage, SpeechSimulatorViewModel>(RoutesHelper.SPEECH_SIMULATOR)
                .AddSingletonWithShellRoute<BaseSettingsPage, BaseSettingsViewModel>($"{RoutesHelper.SETTINGS}/{RoutesHelper.BASE_SETTINGS}")
                .AddSingletonWithShellRoute<DiagnosticPettyPage, DiagnosticPettyViewModel>($"{RoutesHelper.SETTINGS}/{RoutesHelper.DIAGNOSTICS}")
                .AddSingletonWithShellRoute<VoiceSettingsPage, VoiceSettingsViewModel>($"{RoutesHelper.SETTINGS}/{RoutesHelper.VOICE_SETTINGS}");
            return builder;
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<AppShellViewModel>()
                .AddSingleton<RunningTextViewModel>();
            return builder;
        }

        private static MauiAppBuilder RegisterModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<Settings>(services => services.GetService<SettingsService>().Settings.CloneJson());
            return builder;
        }

        private static void AddEditorCustomization()
        {
            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(Editor), (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
            });
        }
    }
}
