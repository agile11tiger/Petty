using CommunityToolkit.Mvvm.Messaging;
using Petty.MessengerCommands.FromPettyGuard;
using Petty.MessengerCommands.ToPettyGuard;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;
using Petty.Resources.Localization;
using Petty.Services.Platforms.PettyCommands;
using Petty.Services.Platforms.Speech;
using Petty.ViewModels.Base;
using System.Text;

namespace Petty.ViewModels
{
    public partial class SpeechSimulatorViewModel : ViewModelBase
    {
        public SpeechSimulatorViewModel(
            IMessenger messenger,
            LoggerService loggerService,
            NavigationService navigationService,
            UserMessagesService userMessagesService,
            LocalizationService localizationService)
            : base(loggerService, navigationService, localizationService)
        {
            _messenger = messenger;
            _userMessagesService = userMessagesService;
            _messenger.Register<SpeechRecognizerResult>(this, OnSpeechReceived);
            _messenger.Register<StartedPettyGuardService>(this, (recipient, message) => SetStartStopButtonTextColor(message.IsStarted));
            _messenger.Register<StoppedPettyGuardService>(this, (recipient, message) => SetStartStopButtonTextColor(!message.IsStopped));
        }

        private readonly IMessenger _messenger;
        private readonly List<string> _sentences = new();
        private readonly UserMessagesService _userMessagesService;
        [ObservableProperty] private bool _isStartingPettyGuardAndroidService;
        [ObservableProperty] private SolidColorBrush _startStopButtonBackground;
        [ObservableProperty] private string _speech = AppResources.UserMessagePettySpeechSimulatorPlaceholder;

        [RelayCommand]
        private async Task StartStopPettyGuardAndroidService()
        {
            if (!IsStartingPettyGuardAndroidService)
                _messenger.Send<StartPettyGuardService>();
            else if (await _userMessagesService.SendRequestAsync(AppResources.UserMessageDisablePettyGuard, AppResources.ButtonNo, AppResources.ButtonYes))
                _messenger.Send<StopPettyGuardService>();
        }

        [RelayCommand]
        private async Task ShowQuestionIconInfo()
        {
            var listNumber = 0;
            var commands = new StringBuilder();
            commands.AppendLine(AppResources.TitlePunctuationWords);
            foreach (var punctuation in PunctuationRecognizer.Punctuations)
            {
                if (punctuation.Key == AppResources.SpeechCommandNewLine)
                    commands.AppendLine($"{listNumber++}. {punctuation.Key} — ");
                else
                    commands.AppendLine($"{listNumber++}. {punctuation.Key} — {punctuation.Value}");
            }

            listNumber = 0;
            commands.AppendLine();
            commands.AppendLine(AppResources.UsefulFeatures);

            foreach (var command in PettyCommandsService.PettyCommands.Values)
                commands.AppendLine($"{listNumber++}. {command.Name} — {command.Description}");

            await _userMessagesService.SendMessageAsync(commands.ToString(), AppResources.ButtonOk, title: AppResources.TitleCommands);
        }

        [RelayCommand]
        private void ClearSpeech()
        {
            Speech = string.Empty;
            _sentences.Clear();
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

        private void SetStartStopButtonTextColor(bool isStarted)
        {
            if (App.Current.Resources.TryGetValue(isStarted ? "GrayButton" : "PrimaryBrush", out object brush))
                StartStopButtonBackground = (SolidColorBrush)brush;
        }
    }
}
