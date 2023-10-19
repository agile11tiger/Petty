using Petty.Resources.Localization;
using Petty.Services.Local.Phone;
using Petty.Services.Local.UserMessages;
using Petty.Services.Platforms.PettyCommands.Commands.Base;
namespace Petty.Services.Platforms.PettyCommands.Commands;

public class BatteryCommand : PettyCommand, IPettyCommand
{
    public BatteryCommand()
    {
        _batteryService = MauiProgram.ServiceProvider.GetService<BatteryService>();
    }

    private readonly BatteryService _batteryService;
    public string Name => AppResources.CommandBattery;
    public string Description => AppResources.CommandBatteryDescription;

    public async Task<bool> TryExecuteAsync()
    {
        await _userMessagesService.SendMessageAsync(AppResources.BatteryCharging, deliveryMode: InformationDeliveryModes.VoiceAlert);
        return true;
    }
}
