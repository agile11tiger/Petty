using Android.Content;
using Petty.Resources.Localization;
using Petty.Services.Local.UserMessages;

namespace Petty.Services.Platforms
{
    public partial class PhoneService
    {
        public async Task<bool> CallAsync(string phoneNumber)
        {
            var packageManager = MauiApplication.Current.BaseContext.PackageManager;
            var telUri = global::Android.Net.Uri.Parse($"tel:{phoneNumber}");
            var callIntent = new Intent(Intent.ActionCall, telUri);
            callIntent.AddFlags(ActivityFlags.NewTask);

            if (string.IsNullOrWhiteSpace(phoneNumber) && callIntent.ResolveActivity(packageManager) == null)
                return await _userMessagesService.SendMessageAsync(AppResources.UserMessageCommandNotAvailable, deliveryMode:InformationDeliveryModes.VoiceAlert);

            MauiApplication.Current.BaseContext.StartActivity(callIntent);
            return true;
        }
    }
}
