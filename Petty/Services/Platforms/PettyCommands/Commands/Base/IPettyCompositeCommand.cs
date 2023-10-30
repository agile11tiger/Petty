namespace Petty.Services.Platforms.PettyCommands.Commands.Base;

public interface IPettyCompositeCommand: IPettyCommand
{
    [Obsolete]
    Task<bool> IPettyCommand.TryExecuteAsync() => throw new NotImplementedException("use TryProcessSpeechAsync(string speech)");
    void Clear();
    Task<bool> TryProcessSpeechAsync(string speech);
}
