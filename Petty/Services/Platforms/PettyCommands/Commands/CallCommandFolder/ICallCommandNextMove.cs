namespace Petty.Services.Platforms.PettyCommands.Commands.CallCommandFolder;

public interface ICallCommandNextMove
{
    States State { get; }
    Task<bool> TryProcessAsync(string speech);
}
