using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.Core.App;
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
        if (!_isStart)
        {
            startForegroundService();
            Toast.MakeText(this, "Служба started", ToastLength.Long).Show();
            if (flags == StartCommandFlags.Retry)
            {
                //not need
            }
        }
        return StartCommandResult.NotSticky;
    }
    public override IBinder OnBind(Intent intent)
    {
        // TODO: Return the communication channel to the service.
        throw new NotImplementedException();
    }


    private void startForegroundService()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O && GetSystemService(Context.NotificationService) is NotificationManager notifcationManager)
            CreateNotificationChannel(notifcationManager);


        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            StartForeground(NOTIFICATION_ID, CreateNotification(), ForegroundService.TypeManifest);
        else
            StartForeground(NOTIFICATION_ID, CreateNotification());
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