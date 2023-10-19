using Petty.Resources.Localization;
namespace Petty.Services.Platforms.PettyCommands.Commands.Base;

public interface IPettyCommand
{
    string Name { get; }
    string Description { get; }
    string ExtendedDescription => null;
    bool NeedFullText => false;
    string CommandText => $"{AppResources.CommandPetName} {Name}";
    bool CheckCommandCompliance(string text) => text.EndsWith(CommandText);
    Task<bool> TryExecuteAsync();
}
