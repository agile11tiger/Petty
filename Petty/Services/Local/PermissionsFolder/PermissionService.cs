using static Microsoft.Maui.ApplicationModel.Permissions;
namespace Petty.Services.Local.PermissionsFolder;

/// <summary>
/// 
/// </summary>
/// <remarks>https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/appmodel/permissions?tabs=android</remarks>
public class PermissionService : Service
{
    private Dictionary<string, Task<PermissionStatus>> _permissions;
    private Dictionary<string, Task<PermissionStatus>> _allPermissions;

    public async Task<Dictionary<string, Task<PermissionStatus>>> GetAllPermissionsAsync()
    {
        _allPermissions = (await GetBasePermissionsAsync()).ToDictionary(entry => entry.Key, entry => entry.Value);
        _allPermissions[nameof(Permissions.Microphone)] = RequestPermissionAsync<Permissions.Microphone>();
        return _allPermissions;
    }

    public async Task<Dictionary<string, Task<PermissionStatus>>> GetBasePermissionsAsync()
    {
        if (_permissions != null)
            return _permissions;

        _permissions = new Dictionary<string, Task<PermissionStatus>>
        {
            [nameof(Permissions.StorageRead)] = RequestPermissionAsync<Permissions.StorageRead>(),
            [nameof(Permissions.StorageWrite)] = RequestPermissionAsync<Permissions.StorageWrite>(),
            [nameof(Permissions.Camera)] = RequestPermissionAsync<Permissions.Camera>(),
            [nameof(Permissions.Flashlight)] = RequestPermissionAsync<Permissions.Flashlight>(),
            [nameof(Permissions.Flashlight)] = RequestPermissionAsync<Permissions.ContactsRead>(),
        };
        await Task.WhenAll(_permissions.Values.ToArray<Task<PermissionStatus>>());
        return _permissions;
    }

    private async Task<PermissionStatus> RequestPermissionAsync<TPermission>() where TPermission : BasePlatformPermission, new()
    {
        var status = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<TPermission>();

        if (status == PermissionStatus.Granted)
            return status;

        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            // Prompt the user to turn on in settings
            // On iOS once a permission has been denied it may not be requested again from the application
            return status;
        }

        status = await Permissions.RequestAsync<TPermission>();
        return status;
    }
}
