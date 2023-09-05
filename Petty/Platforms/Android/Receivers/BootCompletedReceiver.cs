using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Platforms.Android.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    //TODO: bug not working, when its will be working check restart
    //[IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootCompletedReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent != null && intent.Action != null && intent.Action == Intent.ActionBootCompleted)
            {
                App.StartPettyGuadrAndroidService();
            }
        }
    }
}
