using Petty.Resources.Localization;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public class HelpCommand : PettyCommand, IPettyCommand
    {
        public string Name => AppResources.CommandHelp;
        public string Description => AppResources.CommandHelpDescription;

        public Task<bool> TryExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
