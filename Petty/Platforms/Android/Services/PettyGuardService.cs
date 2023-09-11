using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.Core.App;
using Petty;
using Resource = Microsoft.Maui.Resource;

/// <summary>
/// 
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
public class PettyGuardService : Service
{
    public PettyGuardService()
    {
    }

    private static bool _isStart;
    private const string NOTIFICATION_CHANNEL_ID = "999";
    private const int NOTIFICATION_ID = 1;
    private const string NOTIFICATION_CHANNEL_NAME = "notification";
    private const string START_SERVICE = "StartService";
    private const string STOP_SERVICE = "StopService";

    public static void Start()
    {
        var intent = new Android.Content.Intent(START_SERVICE, default, Android.App.Application.Context, typeof(PettyGuardService));
        //TODO: если отзовут разрешение надо потребовать чтобы влючили
        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            Android.App.Application.Context.StartForegroundService(intent);
        else
            Android.App.Application.Context.StartService(intent);
    }

    public static void Stop()
    {
        var intent = new Android.Content.Intent(STOP_SERVICE, default, Android.App.Application.Context, typeof(PettyGuardService));
        Android.App.Application.Context.StopService(intent);
    }

    public override void OnCreate()
    {
        Toast.MakeText(this, "Служба создана", ToastLength.Long).Show();
        base.OnCreate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Toast.MakeText(this, "Служба stopped", ToastLength.Long).Show();
    }

    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        if (intent.Action == START_SERVICE)
            startForegroundService();
        else if (intent.Action == STOP_SERVICE)
            stopForegroundService(startId);

        return StartCommandResult.NotSticky;
    }
    public override IBinder OnBind(Intent intent)
    {
        // TODO: Return the communication channel to the service.
        throw new NotImplementedException();
    }


    private void startForegroundService()
    {
        if (!_isStart)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O && GetSystemService(Context.NotificationService) is NotificationManager notifcationManager)
                CreateNotificationChannel(notifcationManager);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                StartForeground(NOTIFICATION_ID, CreateNotification(), ForegroundService.TypeManifest);
            else
                StartForeground(NOTIFICATION_ID, CreateNotification());

            Toast.MakeText(this, "Служба started", ToastLength.Long).Show();
        }
    }

    private void stopForegroundService(int startId)
    {
        StopForeground(true);
        StopSelf(startId);
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
        notification.SetSmallIcon(Petty.Resource.Mipmap.butterfly_round);
        notification.SetContentTitle("Petty");
        notification.SetContentText("Petty Guard Service is running");
        return notification.Build();
    }
}