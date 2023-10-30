using Petty.Services.Local.UserMessages;
using Service = Petty.Services.Local.Service;

namespace Petty.Services.Platforms;

/// <summary>
/// https://metanit.com/sharp/xamarin/16.1.php
/// </summary>
public partial class PhoneService(UserMessagesService _userMessagesService) : Service
{
#if !ANDROID
    public Task CallAsync(string phone)
    {
        throw new NotImplementedException();
    }
#endif
}
