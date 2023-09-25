using Android.Content;

namespace Petty.Platforms.Android.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    //TODO: why he not added in the manifest automatic??? bug not working, when its will be working check restart
    //[IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootCompletedReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent != null && intent.Action != null && intent.Action == Intent.ActionBootCompleted)
            {
                PettyGuardAndroidService.Start();
            }
        }
    }
}
