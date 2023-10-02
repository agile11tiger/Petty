using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    internal class FlashlightCommand : IPettyCommand
    {
        private bool _isTurnOn;
        public string Name => "flashlight";

        public bool CheckComplianceCommand(string text)
        {
            return text.EndsWith(Name);
        }

        public bool TryExecute()
        {
            Task.Run(async () =>
            {
                try
                {
                    _isTurnOn = !_isTurnOn;

                    if (_isTurnOn)
                        await Flashlight.Default.TurnOnAsync();
                    else
                        await Flashlight.Default.TurnOffAsync();

                }
                catch (FeatureNotSupportedException ex)
                {
                    // Handle not supported on device exception
                }
                catch (PermissionException ex)
                {
                    // Handle permission exception
                }
                catch (Exception ex)
                {
                    // Unable to turn on/off flashlight
                }
            });

            return true;
        }
    }
}
