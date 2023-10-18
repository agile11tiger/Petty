using CommunityToolkit.Mvvm.Messaging;
using Petty.MessengerCommands.FromPettyGuard;
using Petty.MessengerCommands.ToPettyGuard;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;
using Petty.Resources.Localization;
using Petty.Services.Local.UserMessages;
using Petty.Services.Platforms.PettyCommands;
using Petty.Services.Platforms.Speech;
using Petty.ViewModels.Base;
using Petty.ViewModels.Components.DisplayAlert;
using Petty.Views.Components;

namespace Petty.ViewModels
{
    public partial class SpeechSimulatorViewModel : ViewModelBase
    {
        public SpeechSimulatorViewModel(IMessenger messenger, UserMessagesService userMessagesService)
        {
            _messenger = messenger;
            _userMessagesService = userMessagesService;
            _messenger.Register<SpeechRecognizerResult>(this, OnSpeechReceived);
            _messenger.Register<StartedPettyGuardService>(this, (recipient, message) => SetStartStop(message.IsStarted));
            _messenger.Register<StoppedPettyGuardService>(this, (recipient, message) => SetStartStop(!message.IsStopped));
            SetColorStartStopButton();

            _displayAlertPageTask = Task.Run(() => CreateDisplayAlertPageAsync());
        }

        private readonly IMessenger _messenger;
        private readonly List<string> _sentences = [];
        private readonly UserMessagesService _userMessagesService;
        private readonly Task<DisplayAlertPage> _displayAlertPageTask;
        [ObservableProperty] private Color _startStopButtonBackground;
        [ObservableProperty] private bool _isStartingPettyGuardAndroidService;
        [ObservableProperty] private string _speech = AppResources.UserMessagePettySpeechSimulatorPlaceholder;

        [RelayCommand]
        private async Task StartStopPettyGuardAndroidServiceAsync()
        {
            if (!IsStartingPettyGuardAndroidService)
                _messenger.Send<StartPettyGuardService>();
            else if (await _userMessagesService.SendMessageAsync(
                AppResources.UserMessageDisablePettyGuard,
                null,
                AppResources.ButtonNo,
                AppResources.ButtonYes,
                InformationDeliveryModes.DisplayAlertInApp))
                _messenger.Send<StopPettyGuardService>();
        }

        [RelayCommand]
        private async Task ShowQuestionIconInfoAsync()
        {
            await _userMessagesService.SendMessageAsync(await _displayAlertPageTask);
        }

        [RelayCommand]
        private void ClearSpeech()
        {
            Speech = string.Empty;
            _sentences.Clear();
        }

        private async Task<DisplayAlertPage> CreateDisplayAlertPageAsync()
        {
            var listNumber = 0;
            var commands = new List<ILink> { new RawLink(AppResources.TitlePunctuationWords, true) };

            foreach (var punctuation in PunctuationRecognizer.Punctuations)
            {
                if (punctuation.Key == AppResources.SpeechCommandNewLine)
                    commands.Add(new Link([$"{listNumber++}", punctuation.Key, string.Empty]));
                else
                    commands.Add(new Link([$"{listNumber++}", punctuation.Key, punctuation.Value]));
            }

            listNumber = 0;
            commands.Add(new RawLink(string.Empty));
            commands.Add(new RawLink(AppResources.UsefulFeatures, true));

            foreach (var command in PettyCommandsService.PettyCommands.Values)
            {
                if (command.ExtendedDescription != null)
                    commands.Add(new Link([$"{listNumber++}", command.Name, command.Description], async () => 
                        await _userMessagesService.SendMessageAsync(command.ExtendedDescription, $"{AppResources.Command} {command.Name}")));
                else
                    commands.Add(new Link([$"{listNumber++}", command.Name, command.Description]));
            }

            return await _userMessagesService.CreateDisplayAlertPageAsync(commands, AppResources.TitleCommands, AppResources.ButtonOk, null, true);
        }

        private void OnSpeechReceived(object obj, SpeechRecognizerResult speechRecognizerResult)
        {
            _loggerService.Log(speechRecognizerResult.Speech);
            //resultSpeech == finalSpeech 
            //Depends on how to call either because of silence or manually because of the point.
            if (_sentences.Count == 0)
                _sentences.AddRange(new string[] { string.Empty, string.Empty }); //add empty place for resultSpeech and partrialSpeech

            if (speechRecognizerResult.IsPartialSpeech)
                _sentences[^1] = speechRecognizerResult.Speech;

            if (speechRecognizerResult.IsResultSpeech || speechRecognizerResult.IsFinalSpeech)
            {
                _sentences[^1] = string.Empty;//clear partialSpeech
                _sentences[^2] += " " + speechRecognizerResult.Speech; // add to resultSpeech
            }

            if (speechRecognizerResult.IsFinalSpeech)
                _sentences.Add(string.Empty); // add new partialSpeech, previous will be for resultSpeech

            Speech = string.Join(' ', _sentences);
        }

        private void SetStartStop(bool isStarted)
        {
            IsStartingPettyGuardAndroidService = isStarted;
            SetColorStartStopButton();
        }

        private void SetColorStartStopButton()
        {
            if (App.Current.Resources.TryGetValue(IsStartingPettyGuardAndroidService ? "Button" : "Primary", out object color))
                StartStopButtonBackground = (Color)color;
        }
    }
}
