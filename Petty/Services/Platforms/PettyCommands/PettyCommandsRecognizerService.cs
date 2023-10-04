using CommunityToolkit.Mvvm.Messaging;
using Petty.MessengerCommands.FromPettyGuard;
using Petty.Services.Platforms.PettyCommands.Commands;
using Petty.Services.Platforms.Speech;
using System.Reflection;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Petty.Services.Platforms.PettyCommands
{
    public class PettyCommandsService : Service
    {
        public PettyCommandsService(
            IMessenger messenger,
            LoggerService loggerService,
            PettyVoiceService pettyVoiceService,
            SpeechRecognizerService speechRecognizerService)
            : base(loggerService)
        {
            _messenger = messenger;
            _pettyVoiceService = pettyVoiceService;
            _speechRecognizerService = speechRecognizerService;
        }

        static PettyCommandsService()
        {
            PettyCommands = new();
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

        private readonly IMessenger _messenger;
        private readonly PettyVoiceService _pettyVoiceService;
        private readonly static SemaphoreSlim _locker = new(1, 1);
        private readonly SpeechRecognizerService _speechRecognizerService;

        public event Action<IPettyCommand> BroadcastPettyCommand;

        public static Dictionary<string, IPettyCommand> PettyCommands { get; private set; }

        /// <summary>
        /// Try start the petty commands recognizer.
        /// </summary>
        public async Task<bool> TryStartAsync(Action<IPettyCommand> subscriber)
        {
            await _locker.WaitAsync();

            try
            {
                if (BroadcastPettyCommand == null)
                    await _speechRecognizerService.TryStartAsync(OnBroadcastSpeech);

                BroadcastPettyCommand += subscriber;
                return true;
            }
            finally { _locker.Release(); }
        }

        /// <summary>
        /// Stop the petty commands recognizer.
        /// </summary>
        public async Task StopAsync(Action<IPettyCommand> subscriber)
        {
            await _locker.WaitAsync();

            try
            {
                if (BroadcastPettyCommand.GetInvocationList().Length == 1)
                    await _speechRecognizerService.StopAsync(OnBroadcastSpeech);

                BroadcastPettyCommand -= subscriber;
            }
            finally { _locker.Release(); }
        }

        private async void OnBroadcastSpeech(SpeechRecognizerResult speechResult)
        {
            try
            {
                _loggerService.Log(speechResult.Speech);
                speechResult.Speech = speechResult.Speech.AddPunctuation();
                _loggerService.Log($"after {nameof(PunctuationRecognizer.AddPunctuation)}: {speechResult.Speech}");

                var command = RecognizeCommand(speechResult);

                if (command != null)
                    speechResult.NotifyCommandRecognized();
                else
                    return;

                if (await command.TryExecuteAsync())
                {
                    _pettyVoiceService.PlayCommandExecutionSuccessed(command);
                    BroadcastPettyCommand?.Invoke(command);
                }
                else
                    _pettyVoiceService.PlayCommandExecutionFailed(command);

                _messenger.Send(speechResult);
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
            }
        }

        private IPettyCommand RecognizeCommand(SpeechRecognizerResult speechResult)
        {
            foreach (var command in PettyCommands.Values)
            {
                if (command.NeedFullText && !(speechResult.IsResultSpeech || speechResult.IsFinalSpeech))
                    continue;
                
                if (command.CheckCommandCompliance(speechResult.Speech))
                    return command;
            }

            return null;
        }

    }
}
