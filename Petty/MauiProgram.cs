#if ANDROID
using Android.App;
using Petty.Platforms.Android.Services.Audio;
#endif
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Mopups.Interfaces;
using Mopups.Services;
using Petty.Extensions;
using Petty.Helpers;
using Petty.Services.Local.PermissionsFolder;
using Petty.Services.Local.Phone;
using Petty.Services.Local.UserMessages;
using Petty.Services.Platforms;
using Petty.Services.Platforms.PettyCommands;
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
using SpeechEngine.Audio;
using SpeechEngine.Speech;
using SpeechEngine.Services;
using Microsoft.Extensions.DependencyInjection;
using SpeechEngine;

#if ANDROID
//https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/flashlight?tabs=android#:~:text=%5B-,assembly,-%3A%20UsesFeature(%22android.hardware.camera%22
[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]
#endif
namespace Petty;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseSharpnadoTabs(loggerEnable: true, true)
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
    public static IServiceProvider ServiceProvider { get; set; }

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
            .AddSingleton<AudioRecorder>()
            .AddSingleton<SpeechImprover>()
            .AddSingleton<BatteryService>()
            .AddSingleton<DatabaseService>()
            .AddSingleton<SettingsService>()
            .AddSingleton<SpeechRecognizer>()
            .AddSingleton<NavigationService>()
            .AddSingleton<PermissionService>()
            .AddSingleton<WebRequestsService>()
            .AddSingleton<AudioPlayerService>()
            .AddSingleton<LocalizationService>()
            .AddSingleton<UserMessagesService>()
            .AddSingleton<PettyCommandsService>()
            .AddSingleton<NumberParsingService>()
            .AddSingleton<SpeechCommandRecognizer>()
            .AddSingleton<IBattery>(Battery.Default)
            .AddSingleton<ILoggerService, LoggerService>()
            .AddSingleton<IDeviceInfo>(DeviceInfo.Current)
            .AddSingleton<IAudioManager>(AudioManager.Current)
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .AddSingleton<IDeviceDisplay>(DeviceDisplay.Current)
            .AddSingleton<IPopupNavigation>(MopupService.Instance)
            .AddSingleton<IVersionTracking>(VersionTracking.Default)
            .AddSingleton<ISpeechEngineParametersService, SpeechEngineParametersService>()
#if ANDROID
            .AddSingleton<IAudioStream, AudioStream>()
#endif

            .AddTransient<WaveRecorder>();

        return builder;
    }

    private static MauiAppBuilder RegisterPagesWithViewModels(this MauiAppBuilder builder)
    {
        builder.Services
            .AddSingletonWithShellRoute<MainPage, MainViewModel>(RoutesHelper.MAIN)
            .AddTransientWithShellRoute<SettingsPage, SettingsViewModel>(RoutesHelper.SETTINGS) //todo change to singleton, if i do now than touch effect not working
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
