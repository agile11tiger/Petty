using Petty.Services.Local.Phone;
using Petty.Services.Platforms.PettyCommands.Commands.Base;
using SpeechEngine.Services;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Petty.Services.Platforms.PettyCommands.Commands;

public class DialCommand : PettyCommand, IPettyCompositeCommand
{
    public DialCommand()
    {
        _phoneService = MauiProgram.ServiceProvider.GetService<PhoneService>();
        _numberParsingService = MauiProgram.ServiceProvider.GetService<NumberParsingService>();
    }

    private string _phoneNumber;
    private readonly PhoneService _phoneService;
    private readonly NumberParsingService _numberParsingService;
    public string Name => AppResources.CommandDial;
    public string Description => AppResources.CommandDialDescription;

    public async Task<bool> TryProcessSpeechAsync(string speech)
    {

        var strNumbers = speech[speech.LastIndexOf(Name)..];
        var numbers = _numberParsingService.ParseNumbers(strNumbers.Split(' '));
        numbers.ForEach(number => _phoneNumber += number);
        await _userMessagesService.SendVoiceMessageAsync(AppResources.CommandBatteryCharging);
        return true;
    }

    public void Clear()
    {
        
    }
}
