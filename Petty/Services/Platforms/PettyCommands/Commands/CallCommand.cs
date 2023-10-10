using Cyriller;
using Cyriller.Model;
using Petty.Resources.Localization;
using Petty.Services.Platforms.PettyCommands.Commands.Base;
using System.Text.RegularExpressions;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public class CallCommand : PettyCommand, IPettyCommand
    {
        public CallCommand()
        {
            _phoneService = MauiProgram.ServiceProvider.GetService<PhoneService>();
        }

        private string _textContainingTheCommand;
        private readonly CyrName _cyrName = new();
        private readonly PhoneService _phoneService;
        private Dictionary<string, Contact> _contacts;
        private readonly Regex _allLetters = new(@"[^a-zа-яё\s]", RegexOptions.Compiled);
        public bool NeedFullText => true;
        public string Name => AppResources.CommandCall;
        public string Description => AppResources.CommandCallDescription;

        public bool CheckCommandCompliance(string text)
        {
            //Todo: improve, contains is too long
            if (text.Contains((this as IPettyCommand).CommandText))
            {
                _textContainingTheCommand = text;
                return true;
            }

            return false;
        }

        public async Task<bool> TryExecuteAsync()
        {
            try
            {
                if (_contacts == default)
                    await InitialiseContactsAsync();

                var contactName = _textContainingTheCommand[_textContainingTheCommand.LastIndexOf(Name)..];

                if (_contacts.TryGetValue(contactName, out Contact value))
                    _phoneService.Call(value.Phones.First().ToString());
                else
                    await _userMessagesService.SendMessageAsync(_localizationService.Get(nameof(AppResources.UserMessageContactNotFound), contactName), AppResources.ButtonOk);

                return true;
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
            }

            return false;
        }

        private async Task InitialiseContactsAsync()
        {
            _contacts = new();

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
}
