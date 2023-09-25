using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.Core.App;
using CommunityToolkit.Mvvm.Messaging;
using Petty.Services.Local.PettyCommands;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;

/// <summary>
/// https://learn.microsoft.com/ru-ru/xamarin/android/app-fundamentals/services/creating-a-service/
/// </summary>
/// <remarks>
/// https://developer.alexanderklimov.ru/android/theory/services-theory.php
/// https://www.vogella.com/tutorials/AndroidServices/article.html
/// https://www.hellsoft.se/your-guide-to-foreground-services-on-android/
/// https://stackoverflow.com/questions/44425584/context-startforegroundservice-did-not-then-call-service-startforeground?page=1&tab=scoredesc#tab-top
/// https://developer.android.com/about/versions/14/changes/fgs-types-required
/// https://stackoverflow.com/questions/73829758/how-to-create-an-android-foreground-service-in-maui
/// </remarks>
[Service]
public class PettyGuardAndroidService : Service
{
    public PettyGuardAndroidService()
    {
        _pettyCommandsService = MauiApplication.Current.Services.GetService<PettyCommandsService>();
        _messager = MauiApplication.Current.Services.GetService<IMessenger>();
        _loggerService = MauiApplication.Current.Services.GetService<LoggerService>();
    }

    private const int NOTIFICATION_ID = 1;
    private const string STOP_SERVICE = "StopService";
    private const string START_SERVICE = "StartService";
    private const string NOTIFICATION_CHANNEL_ID = "999";
    private const string NOTIFICATION_CHANNEL_NAME = "notification";

    private static bool _isStarting;
    private readonly IMessenger _messager;
    private readonly LoggerService _loggerService;
    private readonly PettyCommandsService _pettyCommandsService;

    public static void Start()
    {
        if (!_isStarting)
        {
            //We ignore the following calls until the initialization of the service is completed,
            //which performs the initialization in a separate thread.
            //After final initialization and readiness of the service, the flag "_isStarting" is synchronized on the client and service.
            //Desynchronization is done so that the client cannot control the state of the service until it is fully initialized.
            //Interrupting initialization is not currently supported.
            _isStarting = true;
            var intent = new Android.Content.Intent(START_SERVICE, default, Android.App.Application.Context, typeof(PettyGuardAndroidService));
            //TODO: ���� ������� ���������� ���� ����������� ����� �������
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                Android.App.Application.Context.StartForegroundService(intent);
            else
                Android.App.Application.Context.StartService(intent);
        }
    }

    public static void Stop()
    {
        if (_isStarting)
        {
            // same logic as for Start()
            _isStarting = false;
            var intent = new Android.Content.Intent(STOP_SERVICE, default, Android.App.Application.Context, typeof(PettyGuardAndroidService));
            Android.App.Application.Context.StopService(intent);
        }
    }

    public override void OnCreate()
    {
        Toast.MakeText(this, "������ �������", ToastLength.Long).Show();
        base.OnCreate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Toast.MakeText(this, "������ stopped", ToastLength.Long).Show();
    }

    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                //This is where initialization is completed and the flag "_isStarting" is synchronized.
                if (intent.Action == START_SERVICE)
                {
                    var isStared = _isStarting = await TryStartForegroundServiceAsync();
                    _messager.Send<StartedPettyGuardService>(new StartedPettyGuardService { IsStarted = isStared });
                }
                else if (intent.Action == STOP_SERVICE)
                {
                    var isStopped = _isStarting = !(await TryStopForegroundServiceAsync(startId));
                    _messager.Send<StoppedPettyGuardService>(new StoppedPettyGuardService { IsStopped = isStopped });
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
            }
        });
        //restart service with last intent, if the service crashed due to memory deficiencies, etc.
        return StartCommandResult.RedeliverIntent;
    }

    public override IBinder OnBind(Intent intent)
    {
        return null;
        // TODO: Return the communication channel to the service???
        //throw new NotImplementedException();
    }

    private async Task<bool> TryStartForegroundServiceAsync()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O && GetSystemService(Context.NotificationService) is NotificationManager notifcationManager)
            CreateNotificationChannel(notifcationManager);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            StartForeground(NOTIFICATION_ID, CreateNotification(), ForegroundService.TypeManifest);
        else
            StartForeground(NOTIFICATION_ID, CreateNotification());

        _pettyCommandsService.BroadcastPettyCommand += OnBroadcastPettyCommand;
        return await _pettyCommandsService.TryStartAsync();
    }

    private void OnBroadcastPettyCommand(Petty.Services.Local.PettyCommands.Commands.IPettyCommand pettyCommand)
    {
        _messager.Send(pettyCommand);
    }

    private async Task<bool> TryStopForegroundServiceAsync(int startId)
    {
        StopForeground(true);
        StopSelf(startId);
        return await _pettyCommandsService.TryStopAsync();
    }

    private void CreateNotificationChannel(NotificationManager notificationManager)
    {
        var channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, NOTIFICATION_CHANNEL_NAME, NotificationImportance.Low);
        notificationManager.CreateNotificationChannel(channel);
    }

    private Notification CreateNotification()
    {
        var notification = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
        notification.SetAutoCancel(false);
        notification.SetOngoing(true);
        //todo: notification.SetSmallIcon(Petty.Resource.Mipmap.butterfly);
        notification.SetContentTitle("Petty");
        notification.SetContentText("Petty Guard Service is running");
        return notification.Build();
    }
}