using Android.Content;

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
            var isAllowed = null != callIntent.ResolveActivity(packageManager);

            if (!string.IsNullOrWhiteSpace(phoneNumber) && isAllowed == true)
            {
                MauiApplication.Current.BaseContext.StartActivity(callIntent);
                return true;
            }

            return false;
        }
    }
}
