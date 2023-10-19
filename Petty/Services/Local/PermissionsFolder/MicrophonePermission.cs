namespace Petty.Services.Local.PermissionsFolder;

public class MicrophonePermission
{
    public static async Task<PermissionStatus> GetAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();

        if (status == PermissionStatus.Granted)
            return status;

        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            // Prompt the user to turn on in settings
            // On iOS once a permission has been denied it may not be requested again from the application
            return status;
        }

        if (Permissions.ShouldShowRationale<Permissions.Microphone>())
        {
            // Prompt the user with additional information as to why the permission is needed
        }

        status = await Permissions.RequestAsync<Permissions.Microphone>();
        return status;
    }
}
