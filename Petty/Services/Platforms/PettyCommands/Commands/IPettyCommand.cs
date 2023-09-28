namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public interface IPettyCommand
    {
        string Name { get; }
        bool CheckComplianceCommand(string text);
        bool TryExecute();
    }
}
