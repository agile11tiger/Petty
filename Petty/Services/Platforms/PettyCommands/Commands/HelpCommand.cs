using Petty.Resources.Localization;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public class HelpCommand : PettyCommand, IPettyCommand
    {
        public string Name => $"{AppResources.CommandPetName} {AppResources.CommandHelp}";

        public bool CheckComplianceCommand(string text)
        {
            return text.EndsWith(Name);
        }

        public Task<bool> TryExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
