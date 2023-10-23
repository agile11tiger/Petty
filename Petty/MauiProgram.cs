using Android.App;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Mopups.Interfaces;
using Mopups.Services;
using Petty.Extensions;
using Petty.Helpers;
using Petty.Platforms.Android.Services.Audio;
using Petty.Services.Local.PermissionsFolder;
using Petty.Services.Local.Phone;
using Petty.Services.Local.UserMessages;
using Petty.Services.Platforms;
using Petty.Services.Platforms.Audio;
using Petty.Services.Platforms.PettyCommands;
using Petty.Services.Platforms.Speech;
using Petty.ViewModels.Settings;
using Petty.Views;
using Petty.Views.Controls;
using Petty.Views.Controls.Magic;
using Petty.Views.Controls.YinYangSpinner;
using Petty.Views.Settings;
using Plugin.Maui.Audio;
using Sharpnado.Tabs;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SkiaSharp.Views.Maui.Handlers;

//https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/flashlight?tabs=android#:~:text=%5B-,assembly,-%3A%20UsesFeature(%22android.hardware.camera%22
[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]
namespace Petty;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseSharpnadoTabs(loggerEnable: false)
            .UseMauiCommunityToolkit()
            //todo https://stackoverflow.com/questions/72463558/how-to-play-an-audio-file-net-maui
            //.UseMauiCommunityToolkitMediaElement()  https://stackoverflow.com/questions/75525722/correct-way-to-set-net-maui-mediaelement-source-from-code
            .ConfigureMopups()
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<YinYangSpinnerSKCanvasView, SKCanvasViewHandler>();
                handlers.AddHandler<GradientView, SKCanvasViewHandler>();
            })
            .ConfigureEssentials(essentials =>
            {
                essentials.UseVersionTracking();
            })
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
        _loggerService = ServiceProvider.GetService<LoggerService>();
        return app;
    }

    private static LoggerService _loggerService;
    public static IServiceProvider ServiceProvider { get; private set; }

    private static void HandleUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        _loggerService.Log(e.Exception, LogLevel.Critical);
        e.SetObserved();
    }

    private static void HandleCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        _loggerService.Log((Exception)e.ExceptionObject, LogLevel.Critical);
    }

    private static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder)
    {
        builder.Services
            .AddSingleton<VoiceService>()
            .AddSingleton<PhoneService>()
            .AddSingleton<LoggerService>()
            .AddSingleton<BatteryService>()
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
            .AddSingleton<IBattery>(Battery.Default)
            .AddSingleton<IDeviceInfo>(DeviceInfo.Current)
            .AddSingleton<IAudioManager>(AudioManager.Current)
            .AddSingleton<IDeviceDisplay>(DeviceDisplay.Current)
            .AddSingleton<IPopupNavigation>(MopupService.Instance)
            .AddSingleton<IVersionTracking>(VersionTracking.Default)

            .AddTransient<WaveRecorderService>();

        return builder;
    }

    private static MauiAppBuilder RegisterPagesWithViewModels(this MauiAppBuilder builder)
    {
        builder.Services
            .AddSingletonWithShellRoute<MainPage, MainViewModel>(RoutesHelper.MAIN)
            .AddSingletonWithShellRoute<SettingsPage, SettingsViewModel>(RoutesHelper.SETTINGS)
            .AddSingletonWithShellRoute<SpeechSimulatorPage, SpeechSimulatorViewModel>(RoutesHelper.SPEECH_SIMULATOR)
            .AddTransientWithShellRoute<BaseSettingsPage, BaseSettingsViewModel>($"{RoutesHelper.SETTINGS}/{RoutesHelper.BASE_SETTINGS}")
            .AddTransientWithShellRoute<DiagnosticPettyPage, DiagnosticPettyViewModel>($"{RoutesHelper.SETTINGS}/{RoutesHelper.DIAGNOSTICS}")
            .AddTransientWithShellRoute<VoiceSettingsPage, VoiceSettingsViewModel>($"{RoutesHelper.SETTINGS}/{RoutesHelper.VOICE_SETTINGS}");
        return builder;
    }

    private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services
            .AddSingleton<AppShellViewModel>()
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
