using Petty.Resources.Localization;
using Petty.Services.Platforms.Paths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public class CallCommand : PettyCommand, IPettyCommand
    {
        public CallCommand()
        {
            _phoneService = MauiProgram.ServiceProvider.GetService<PhoneService>();
        }

        private string _textContainingTheCommand;
        private readonly PhoneService _phoneService;
        private Dictionary<string, Contact> _contacts;
        public bool NeedFullText => true;
        public string Name => AppResources.CommandCall;

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
                {
                    _contacts = new();

                    foreach (var contact in await Contacts.Default.GetAllAsync())
                    {
                        var name = CultureInfo.CurrentCulture.Name == "ru-RU" 
                            ? Regex.Replace(contact.DisplayName.ToLower(), @"[^а-яё\s]", "", RegexOptions.Compiled)
                            : Regex.Replace(contact.DisplayName.ToLower(), @"[^a-z\s]", "", RegexOptions.Compiled);

                         _contacts[name] = contact;
                    }

                    var contactName = _textContainingTheCommand[_textContainingTheCommand.LastIndexOf(Name)..];

                    if (_contacts.TryGetValue(contactName, out Contact value))
                        _phoneService.Call(value.Phones.First().ToString());
                    else
                        await _userMessagesService.SendMessageAsync(AppResources.UserMessageContactNotFound, AppResources.ButtonOk);

                }

                return true;
            }
            catch(Exception ex)
            {
                _loggerService.Log(ex);
            }

            return false;
        }
    }
}
