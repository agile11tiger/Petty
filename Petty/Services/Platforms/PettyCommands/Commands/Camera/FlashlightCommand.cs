using Petty.Resources.Localization;
using Petty.Services.Platforms.PettyCommands.Commands.Base;
namespace Petty.Services.Platforms.PettyCommands.Commands.Camera;

public class FlashlightCommand : PettyCommand, IPettyCommand
{
    private bool _isTurnOn;
    public string Name => AppResources.CommandFlashlight;
    public string Description => AppResources.CommandFlashlightDescription;
    public string ExtendedDescription => _localizationService.IsRussianLanguage ? AppResources.CommandFlashlightExtendedDescription : null;

    public async Task<bool> TryExecuteAsync()
    {
        try
        {
            _isTurnOn = !_isTurnOn;

            if (_isTurnOn)
                await Flashlight.Default.TurnOnAsync();
            else
                await Flashlight.Default.TurnOffAsync();

            return true;
        }
        catch (FeatureNotSupportedException ex)
        {
            _loggerService.Log(ex);
            await _userMessagesService.SendMessageAsync(AppResources.UserMessageCommandNotSupported);
        }
        catch (Exception ex)
        {
            _loggerService.Log(ex);
        }

        return false;
    }
}
