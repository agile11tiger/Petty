using CommunityToolkit.Mvvm.Messaging;
using Petty.ViewModels.Base;
using Petty.Resources.Localization;
using Petty.MessengerCommands.FromPettyGuard;
using Petty.Services.Platforms.Speech;
using System.Text;

namespace Petty.ViewModels
{
    public partial class DiagnosticPettyViewModel : ViewModelBase
    {
        public DiagnosticPettyViewModel(
            IMessenger messenger,
            LoggerService loggerService,
            MainViewModel mainViewModel,
            NavigationService navigationService,
            UserMessagesService userMessagesService,
            LocalizationService localizationService)
            : base(loggerService, navigationService, localizationService)
        {
            _messenger = messenger;
            _mainViewModel = mainViewModel;
            _userMessagesService = userMessagesService;
            _messenger.Register<SpeechRecognizerResult>(this, OnSpeechReceived);
        }

        private readonly IMessenger _messenger;
        private readonly MainViewModel _mainViewModel;
        private readonly List<string> _sentences = new() { string.Empty };
        private readonly UserMessagesService _userMessagesService;
        [ObservableProperty] private string _speech = "Здесь будет ваша речь";
        [ObservableProperty] private bool _isStartingPettyGuardAndroidService;

        [RelayCommand]
        private async Task StartStopPettyGuardAndroidService()
        {
            await _mainViewModel.StartStopPettyGuardAndroidServiceCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task ShowQuestionIconInfo()
        {
            var commands = new StringBuilder();
            var listNumber = 0;

            foreach (var punctuation in PunctuationRecognizer.Punctuations)
            {
                if (punctuation.Key == AppResources.NewLine)
                    commands.AppendLine($"{listNumber++}. {punctuation.Key} — ");
                else
                    commands.AppendLine($"{listNumber++}. {punctuation.Key} — {punctuation.Value}");
            }

            await _userMessagesService.SendMessageAsync(commands.ToString(), AppResources.Ok, title:AppResources.PunctuationWords);
        }

        private void OnSpeechReceived(object obj, SpeechRecognizerResult speechRecognizerResult)
        {
            if (speechRecognizerResult.Speech.Last() == '.')
            {
                _sentences[^1] = speechRecognizerResult.Speech;
                _sentences.Add(string.Empty);
            }
            else
                _sentences[_sentences.Count - 1] = speechRecognizerResult.Speech;

            Speech = string.Join(' ', _sentences);
        }

        [RelayCommand]
        public void ClearSpeech()
        {
            Speech = string.Empty;
        }
    }
}
