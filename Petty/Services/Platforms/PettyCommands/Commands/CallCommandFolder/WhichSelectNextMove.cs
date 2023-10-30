using Petty.Resources.Localization;
using Petty.Services.Local.UserMessages;
using SpeechEngine.Services;
namespace Petty.Services.Platforms.PettyCommands.Commands.CallCommandFolder;

public class WhichSelectNextMove(
    Contact _contact,
    PhoneService _phoneService,
    UserMessagesService _userMessagesService,
    NumberParsingService _numberParsingService)
    : ICallCommandNextMove
{
    public States State => States.ContactFoundWhichSelect;

    public async Task<bool> TryProcessAsync(string speech)
    {
        if (_numberParsingService.TryParseOrdinalNumber(speech[(speech.LastIndexOf(' ') + 1)..], out int number))
        {
            if (number >= _contact.Phones.Count)
                await _userMessagesService.SendMessageAsync(AppResources.CommandCallSelectNumberFromList, deliveryMode: InformationDeliveryModes.VoiceAlert);
            else
            {
                await _phoneService.CallAsync(_contact.Phones[number].ToString());
                return true;
            }
        }

        return false;
    }
}