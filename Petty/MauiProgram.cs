﻿using Android.App;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Petty.Extensions;
using Petty.Helpres;
using Petty.Services.Local.PermissionsFolder;
using Petty.Services.Platforms.Audio;
using Petty.Services.Platforms.PettyCommands;
using Petty.Services.Platforms.Speech;
using Petty.ViewModels.Components;
using Petty.ViewModels.Components.GraphicsViews;
using Petty.Views;
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
                //.UseMauiCommunityToolkitMediaElement()
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
            builder.Services
                .AddSingleton<LoggerService>()
                .AddSingleton<DatabaseService>()
                .AddSingleton<SettingsService>()
                .AddSingleton<PettyVoiceService>()
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
                .AddSingletonWithShellRoute<SettingsPage, SettingsViewModel>(RoutesHelper.SETINGS)
                .AddSingletonWithShellRoute<SpeechSimulatorPage, SpeechSimulatorViewModel>(RoutesHelper.SPEECH_SIMULATOR)
                .AddSingletonWithShellRoute<DiagnosticPettyPage, DiagnosticPettyViewModel>(
                    $"{RoutesHelper.SETINGS}/{RoutesHelper.DIAGNOSTICS}");
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
    }
}
