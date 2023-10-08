using Petty.Resources.Localization;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public class FlashlightCommand : PettyCommand, IPettyCommand
    {
        private bool _isTurnOn;
        public string Name => AppResources.CommandFlashlight;
        public string Description => AppResources.CommandFlashlightDescription;

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
                await _userMessagesService.SendMessageAsync(AppResources.UserMessageCommandNotSupported, AppResources.ButtonOk);
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
            }

            return false;
        }
    }
}
