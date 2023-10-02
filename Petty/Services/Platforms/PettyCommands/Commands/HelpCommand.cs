﻿namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public class HelpCommand : IPettyCommand
    {
        public string Name => "help";

        public bool CheckComplianceCommand(string text)
        {
            return text.EndsWith(Name);
        }

        public bool TryExecute()
        {
            throw new NotImplementedException();
        }
    }
}
