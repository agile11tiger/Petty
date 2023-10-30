using CommunityToolkit.Mvvm.Messaging;
using Petty.Resources.Localization;
using Petty.Services.Local.UserMessages;
using Petty.Services.Platforms.PettyCommands.Commands.Base;
using SpeechEngine.Audio;
using SpeechEngine.Speech;
using System.Reflection;
namespace Petty.Services.Platforms.PettyCommands;

public class PettyCommandsService(
    IMessenger _messenger,
    VoiceService _voiceService, 
    SpeechImprover _speechImprover,
    SpeechRecognizer _speechRecognizerService)
    : Service
{
    static PettyCommandsService()
    {
        PettyCommands = [];
        var iPettyCommandType = typeof(IPettyCommand);
        var currentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var currentAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName.StartsWith(currentAssemblyName));
        var types = currentAssembly.GetTypes().Where(p => iPettyCommandType.IsAssignableFrom(p) && p.IsClass);

        foreach (var type in types)
        {
            var command = (IPettyCommand)Activator.CreateInstance(type);
            PettyCommands[command.CommandText] = command;
        }
    }

    private IPettyCompositeCommand _currentPettyCommand;
    private readonly static SemaphoreSlim _locker = new(1, 1);
    private readonly HashSet<string> _commandCancelWords = [AppResources.CommandWordCancel, AppResources.ButtonNo];

    public static Dictionary<string, IPettyCommand> PettyCommands { get; private set; }

    /// <summary>
    /// Try start the petty commands recognizer.
    /// </summary>
    public async Task<bool> TryStartAsync()
    {
        await _locker.WaitAsync();

        try
        {
            return await _speechRecognizerService.TryStartAsync(OnBroadcastSpeech);

        }
        finally { _locker.Release(); }
    }

    /// <summary>
    /// Stop the petty commands recognizer.
    /// </summary>
    public async Task StopAsync()
    {
        await _locker.WaitAsync();

        try
        {
            await _speechRecognizerService.StopAsync(OnBroadcastSpeech);
        }
        finally { _locker.Release(); }
    }

    private async void OnBroadcastSpeech(SpeechRecognizerResult speechResult)
    {
        try
        {
            var command = RecognizeCommand(speechResult);

            if (!await TryProcessCompositeCommandAsync(speechResult, command))
                await ProcessCommandAsync(speechResult, command);

            _speechImprover.Improve(speechResult);
            _messenger.Send(speechResult);
        }
        catch (Exception ex)
        {
            _loggerService.Log(ex);
        }
    }

    private async Task<bool> TryProcessCompositeCommandAsync(SpeechRecognizerResult speechResult, IPettyCommand command)
    {
        if (_currentPettyCommand == null && command is IPettyCompositeCommand compositeCommand)
            _currentPettyCommand = compositeCommand;

        if (_currentPettyCommand != null)
        {
            foreach (var cancelWord in _commandCancelWords)
                if (speechResult.Speech.EndsWith(cancelWord))
                {
                    FinishCompositeCommand(speechResult);
                    return true;
                }

            if (await _currentPettyCommand.TryProcessSpeechAsync(speechResult.Speech))
                FinishCompositeCommand(speechResult);

            return true;
        }

        return false;
    }

    private void FinishCompositeCommand(SpeechRecognizerResult speechResult)
    {
        _currentPettyCommand = null;
        _currentPettyCommand.Clear();
        speechResult.NotifyCommandRecognized();
    }

    private async Task ProcessCommandAsync(SpeechRecognizerResult speechResult, IPettyCommand command)
    {
        if (command != null)
        {
            speechResult.NotifyCommandRecognized();

            if (await command.TryExecuteAsync())
                _voiceService.PlayCommandExecutionSuccessed(command);
            else
                _voiceService.PlayCommandExecutionFailed(command);
        }
    }

    private IPettyCommand RecognizeCommand(SpeechRecognizerResult speechResult)
    {
        if (_currentPettyCommand == null && (speechResult.IsResultSpeech || speechResult.IsFinalSpeech))
            foreach (var command in PettyCommands.Values)
                if (command.CheckCommandCompliance(speechResult.Speech))
                    return command;

        return null;
    }
}
