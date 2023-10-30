using CommunityToolkit.Mvvm.Messaging;
using Petty.MessengerCommands.ToPettyGuard;
using Petty.PlatformsShared.MessengerCommands.FromPettyGuard;
using Petty.Resources.Localization;
using Petty.Services.Local.UserMessages;
using Petty.Services.Platforms.PettyCommands;
using Petty.ViewModels.DisplayAlert;
using Sharpnado.Tabs.Effects;
using SpeechEngine.Speech;
namespace Petty.ViewModels;

public partial class SpeechSimulatorViewModel : ViewModelBase
{
    public SpeechSimulatorViewModel(IMessenger messenger, UserMessagesService userMessagesService)
    {
        _messenger = messenger;
        _userMessagesService = userMessagesService;
        SetColorStartStopButton();
        _displayAlertPageTask = Task.Run(() => CreateDisplayAlertPageAsync());
    }

    private readonly IMessenger _messenger;
    private readonly UserMessagesService _userMessagesService;
    private readonly Task<DisplayAlertPage> _displayAlertPageTask;
    [ObservableProperty] private Color _startStopButtonBackground;
    [ObservableProperty] private bool _isStartingPettyGuardAndroidService;
    [ObservableProperty] private string _speech = AppResources.UserMessagePettySpeechSimulatorPlaceholder;

    [RelayCommand]
    private void Appearing()
    {
        //await Task.Delay(10); //Otherwise, the question mark is shown before moving to a new page.
        _appShellViewModel.Title = AppResources.PageSpeechSimulator;
        _appShellViewModel.IsVisibleQuestionIcon = true;
        _appShellViewModel.ShowQuestionIconInfo = ShowQuestionIconInfoCommand;
        _messenger.Register<SpeechRecognizerResult>(this, OnSpeechReceived);
        _messenger.Register<StartedPettyGuardService>(this, (recipient, message) => SetStartStop(message.IsStarted));
        _messenger.Register<StoppedPettyGuardService>(this, (recipient, message) => SetStartStop(!message.IsStopped));
    }

    [RelayCommand]
    private void Disappearing()
    {
        _appShellViewModel.IsVisibleQuestionIcon = false;
        _appShellViewModel.ShowQuestionIconInfo = null;
        _messenger.Unregister<SpeechRecognizerResult>(this);
        _messenger.Unregister<StartedPettyGuardService>(this);
        _messenger.Unregister<StoppedPettyGuardService>(this);
    }

    [RelayCommand]
    private async Task StartStopPettyGuardAndroidServiceAsync()
    {
        HapticFeedback();

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
        HapticFeedback();
        await _userMessagesService.SendMessageAsync(await _displayAlertPageTask);
    }

    private async Task<DisplayAlertPage> CreateDisplayAlertPageAsync()
    {
        var commands = new List<ILink>() { new RawLink(SpeechCommandRecognizer.SpeechCommandIntroduction) };
        AddSpeechCommand(commands);
        AddRemainingCommands(commands);
        return await _userMessagesService.CreateDisplayAlertPageAsync(commands, AppResources.CommandsTitle, AppResources.ButtonOk, null, true);
    }
    
    private void AddSpeechCommand(List<ILink> commands)
    {
        var listNumber = 0;
        commands.Add(new RawLink(string.Empty));
        commands.Add(new RawLink(SpeechCommandRecognizer.SpeechCommandTitle, isTitle: true));

        foreach (var command in SpeechCommandRecognizer.Commands)
            commands.Add(new Link([command.Key, command.Value], listNumber++));

        foreach (var punctuation in SpeechCommandRecognizer.Punctuations)
        {
            if (punctuation.Value == "\r\n")
                commands.Add(new Link([punctuation.Key, string.Empty], listNumber++));
            else
                commands.Add(new Link([punctuation.Key, punctuation.Value], listNumber++));
        }
    }

    private void AddRemainingCommands(List<ILink> commands)
    {
        var listNumber = 0;
        commands.Add(new RawLink(string.Empty));
        commands.Add(new RawLink(AppResources.CommandsTitleUsefulFeatures, isTitle: true));

        foreach (var command in PettyCommandsService.PettyCommands.Values)
        {
            if (command.ExtendedDescription != null)
                commands.Add(new Link([command.Name, command.Description], listNumber++, async () =>
                    await _userMessagesService.SendMessageAsync(command.ExtendedDescription, $"{AppResources.Command} {command.Name}")));
            else
                commands.Add(new Link([command.Name, command.Description], listNumber++));
        }
    }

    private void OnSpeechReceived(object obj, SpeechRecognizerResult speechRecognizerResult)
    {
        Speech = speechRecognizerResult.Speech;
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
