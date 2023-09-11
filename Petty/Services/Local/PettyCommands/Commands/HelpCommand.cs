using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Petty.Services.Local.PettyCommands.Commands
{
    public class HelpCommand : IPettyCommand
    {
        public string Name => "help";

        public bool CheckComplianceCommand(string text)
        {
            return text.Contains(Name);
        }

        public bool TryExecute()
        {
            throw new NotImplementedException();
        }
    }
}
