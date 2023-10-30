using Cyriller;
using Cyriller.Model;
using Petty.Services.Platforms.PettyCommands.Commands.Base;
using SpeechEngine.Services;
using System.Text.RegularExpressions;
namespace Petty.Services.Platforms.PettyCommands.Commands.CallCommandFolder;

public class CallCommand : PettyCommand, IPettyCompositeCommand
{
    public CallCommand()
    {
        _phoneService = MauiProgram.ServiceProvider.GetService<PhoneService>();
        _numberParsingService = MauiProgram.ServiceProvider.GetService<NumberParsingService>();

        if (_localizationService.IsRussianLanguage)
        {
            _contacts = [];
            _cyrName = new();
        }
    }

    private readonly CyrName _cyrName;
    private readonly PhoneService _phoneService;
    private ICallCommandNextMove _callCommandNextMove;
    private readonly Dictionary<string, Contact> _contacts;
    private readonly NumberParsingService _numberParsingService;
    private readonly Regex _allLetters = new(@"[^a-zа-яё\s]", RegexOptions.Compiled);
    public bool IsStopSendingData { get; set; }
    public string Name => AppResources.CommandCall;
    public string Description => AppResources.CommandCallDescription;
    public string ExtendedDescription => _localizationService.IsRussianLanguage ? AppResources.CommandCallExtendedDescription : null;

    public bool CheckCommandCompliance(string text)
    {
        if (text.Split().Length < 3) //if less than "petty call someDisplayName"
            return false;
        //todo maybe can improve?
        return text.Contains((this as IPettyCommand).CommandText);
    }

    public async Task<bool> TryProcessSpeechAsync(string speech)
    {
        try
        {
            if (_contacts.Count == 0)
                await InitializeContactsAsync();

            if (_callCommandNextMove != null)
            {
                if (await _callCommandNextMove.TryProcessAsync(speech))
                {
                    Clear();
                    return true;
                }

                return false;
            }

            var contactName = speech[speech.LastIndexOf(Name)..];

            if (_contacts.TryGetValue(contactName, out Contact contact))
            {
                if (contact.Phones.Count == 0)
                    await _userMessagesService.SendVoiceMessageAsync(_localizationService.Get(nameof(AppResources.CommandCallNotPhoneNumbers), contactName));
                else if (contact.Phones.Count == 1)
                    await _phoneService.CallAsync(contact.Phones.First().ToString()); //todo: отправить какой нибудь дисплей с инфой о контакте
                else
                {
                    await _userMessagesService.SendVoiceMessageAsync(_localizationService.Get(nameof(AppResources.CommandCallContactHaveSeveralPhoneNumbers), contactName));
                    _callCommandNextMove = new WhichSelectNextMove(contact, _phoneService, _userMessagesService, _numberParsingService);
                }
            }
            else
                await _userMessagesService.SendVoiceMessageAsync(_localizationService.Get(nameof(AppResources.CommandCallContactNotFound), contactName));

        }
        catch (Exception ex)
        {
            await _userMessagesService.SendVoiceMessageAsync(AppResources.CommandExceptionInCode);
            _loggerService.Log(ex);
        }

        //It doesn’t matter what happened, if the code execution command has reached here, then the command is completed.
        return true;
    }

    public void Clear()
    {
        _callCommandNextMove = null;
    }

    private async Task InitializeContactsAsync()
    {
        foreach (var contact in await Contacts.Default.GetAllAsync())
        {
            try
            {
                var displayName = _allLetters.Replace(contact.DisplayName.ToLower(), " ");
                _contacts[displayName] = contact;

                if (_localizationService.IsRussianLanguage)
                {
                    var displayNameParts = displayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var name = displayNameParts[0];
                    var surname = displayNameParts.Length > 1 ? displayNameParts[1] : null;
                    var result = _cyrName.Decline(name, surname, null, CasesEnum.Dative);
                    displayNameParts[0] = result.Name;

                    if (displayNameParts.Length > 1)
                        displayNameParts[1] = result.Surname;

                    displayName = string.Join(' ', displayNameParts).ToLower();
                    _contacts[displayName] = contact;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log(contact.DisplayName, ex);
            }
        }
    }
}
