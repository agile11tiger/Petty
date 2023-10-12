using CommunityToolkit.Mvvm.Messaging;
using Petty.Resources.Localization;
using Petty.Services.Platforms.PettyCommands.Commands.Base;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public class DeviceInfoCommand : PettyCommand, IPettyCommand
    {
        public DeviceInfoCommand()
        {
            _deviceInfo = MauiProgram.ServiceProvider.GetService<IDeviceInfo>();
            _deviceDisplayInfo = MauiProgram.ServiceProvider.GetService<IDeviceDisplay>().MainDisplayInfo;
        }

        private readonly IDeviceInfo _deviceInfo;
        private readonly DisplayInfo _deviceDisplayInfo;

        public string Name => AppResources.CommandDeviceInfo;
        public string Description => AppResources.CommandDeviceInfoDescription;

        public async Task<bool> TryExecuteAsync()
        {
            try
            {
                var deviceInfo = new List<string>
                {
                    $"{AppResources.DeviceInfoModel}: {_deviceInfo.Model}",
                    $"{AppResources.DeviceInfoPlatform}: {_deviceInfo.Platform}",
                    $"{AppResources.DeviceInfoDensity}: {_deviceDisplayInfo.Density}",
                    $"{AppResources.DeviceInfoOSVersion}: {_deviceInfo.VersionString}",
                    $"{AppResources.DeviceInfoManufacturer}: {_deviceInfo.Manufacturer}",
                    $"{AppResources.DeviceInfoWidth}: {_deviceDisplayInfo.Width} {AppResources.DeviceInfoPixel}",
                    $"{AppResources.DeviceInfoHeight}: {_deviceDisplayInfo.Height} {AppResources.DeviceInfoPixel}",
                    $"{AppResources.DeviceInfoRefreshRate}: {(int)_deviceDisplayInfo.RefreshRate} {AppResources.DeviceInfoHertz}"
                };

                var deviceInfoMessage = string.Join("\r\n", deviceInfo.OrderBy(d => d.Length));
                await _userMessagesService.SendMessageAsync(deviceInfoMessage, AppResources.ButtonOk, title: AppResources.DeviceInfo);
                return true;
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
            }

            return false;
        }
    }
}
