using CommunityToolkit.Mvvm.Messaging;
using Petty.Services.Local.UserMessages;
using SpeechEngine;
using SpeechEngine.Audio;

namespace Petty.Services.Platforms.PettyCommands;

public class SpeechEngineParametersService(IMessenger _messenger, UserMessagesService _userMessagesService)
    : ISpeechEngineParametersService
{
    public async Task<bool> CheckBeforeDownloadingSpeechModelAsync()
    {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.None)
            return await _userMessagesService.SendMessageAsync(AppResources.UserMessageCheckNetworkConnection);

        if (!await _userMessagesService.SendMessageAsync(
            AppResources.UserMessageDownloadVoskModelMessage,
            AppResources.TitleDownloading,
            AppResources.ButtonLater,
            AppResources.ButtonDownload,
            InformationDeliveryModes.DisplayAlertInApp))
            return false;

        return true;
    }

    public void UpdateDownloadingBar(SpeechModelDownloadingProgressBar downloadingProgressBar)
    {
        _messenger.Send(downloadingProgressBar);
    }

    public async Task SendTryLaterAsync()
    {
        await _userMessagesService.SendMessageAsync(AppResources.UserMessageTryLater);
    }
}
