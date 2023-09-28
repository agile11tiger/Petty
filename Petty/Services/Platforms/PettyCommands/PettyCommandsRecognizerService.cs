using CommunityToolkit.Mvvm.Messaging;
using Petty.MessengerCommands.FromPettyGuard;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;
using Petty.Services.Platforms.PettyCommands.Commands;
using Petty.Services.Platforms.Speech;
using System.Reflection;

namespace Petty.Services.Platforms.PettyCommands
{
    public class PettyCommandsService
    {
        public PettyCommandsService(
            IMessenger messenger,
            PettyVoiceService pettyVoiceService,
            SpeechRecognizerService speechRecognizerService)
        {
            _messenger = messenger;
            _pettyVoiceService = pettyVoiceService;
            _speechRecognizerService = speechRecognizerService;
            var iPettyCommandType = typeof(IPettyCommand);
            var currentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var currentAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName.StartsWith(currentAssemblyName));
            var types = currentAssembly.GetTypes().Where(p => iPettyCommandType.IsAssignableFrom(p) && p.IsClass);

            foreach (var type in types)
            {
                var command = (IPettyCommand)Activator.CreateInstance(type);
                _pettyCommands[command.Name] = command;
            }
        }

        private readonly IMessenger _messenger;
        private readonly PettyVoiceService _pettyVoiceService;
        private readonly static SemaphoreSlim _locker = new(1, 1);
        private readonly SpeechRecognizerService _speechRecognizerService;
        private readonly Dictionary<string, IPettyCommand> _pettyCommands = new();

        private event Action<IPettyCommand> BroadcastPettyCommand;

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

        private void OnBroadcastSpeech(SpeechRecognizerResult speechResult)
        {
            _messenger.Send(new SpeechRecognizerResult() { Speech = speechResult.Speech.AddPunctuation() });
            return;
            


            var command = RecognizeCommand(speechResult.Speech);

            if (command != null)
                speechResult.IsCommandRecognized = true;
            else
                return;

            if (command.TryExecute())
            {
                _pettyVoiceService.PlayCommandExecutionSuccessed(command);
                BroadcastPettyCommand?.Invoke(command);
            }
            else
                _pettyVoiceService.PlayCommandExecutionFailed(command);
        }

        private IPettyCommand RecognizeCommand(string speech)
        {
            foreach (var command in _pettyCommands.Values)
                if (command.CheckComplianceCommand(speech))
                    return command;

            return null;
        }
    }
}
