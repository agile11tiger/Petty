using Android.Content;
using Android.Content.PM;
using Android.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Platforms
{
    public partial class PhoneService
    {
        public bool Call(string phoneNumber)
        {
            var packageManager = MauiApplication.Current.BaseContext.PackageManager;
            var telUri = global::Android.Net.Uri.Parse($"tel:{phoneNumber}");
            var callIntent = new Intent(Intent.ActionCall, telUri);
            callIntent.AddFlags(ActivityFlags.NewTask);
            var result = null != callIntent.ResolveActivity(packageManager);

            if (!string.IsNullOrWhiteSpace(phoneNumber) && result == true)
            {
                MauiApplication.Current.BaseContext.StartActivity(callIntent);
                return true;
            }

            return false;
        }
    }
}
