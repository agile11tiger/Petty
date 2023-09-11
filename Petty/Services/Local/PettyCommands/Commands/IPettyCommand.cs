using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Local.PettyCommands.Commands
{
    public interface IPettyCommand
    {
        string Name { get; }
        bool CheckComplianceCommand(string text);
        bool TryExecute();
    }
}
