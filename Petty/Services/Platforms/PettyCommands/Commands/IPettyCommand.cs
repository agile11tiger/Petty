using Petty.Resources.Localization;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public interface IPettyCommand
    {
        string Name { get; }
        bool NeedFullText => false;
        string CommandText => $"{AppResources.CommandPetName} {Name}";
        bool CheckCommandCompliance(string text)
        {
            return text.EndsWith(CommandText);
        }
        Task<bool> TryExecuteAsync();
    }
}
