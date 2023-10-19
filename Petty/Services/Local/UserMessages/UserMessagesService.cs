using Mopups.Interfaces;
using Mopups.Pages;
using Petty.Resources.Localization;
using Petty.ViewModels.Components.DisplayAlert;
using Petty.Views.Components;
namespace Petty.Services.Local.UserMessages;

public class UserMessagesService(VoiceService _voiceService, IPopupNavigation _popupNavigation) : Service
{
    /// <summary>
    /// Send message to user
    /// </summary>
    /// <returns>Default return false. This means that the user did not click the Accept button.
    /// Thus, if he clicks on the accept button, it will return true.
    /// </returns>
    public async Task<bool> SendMessageAsync(
        string message,
        string title = null,
        string cancel = "Ok",
        string accept = null,
        InformationDeliveryModes deliveryMode = InformationDeliveryModes.DisplayAlertInApp)
    {
        if (cancel == "Ok")
            cancel = AppResources.ButtonOk;

        if (deliveryMode == InformationDeliveryModes.DisplayAlertInApp)
            return await SendMessageAsync(await CreateDisplayAlertPageAsync([new RawLink(message)], title, cancel, accept, false));
        else if (deliveryMode == InformationDeliveryModes.DisplayAlertOutsideApp)
            ShowOutsideApplicationAsync();
        else if (deliveryMode == InformationDeliveryModes.DisplayAlertOutsideAppOnLockedScreen)
            ShowOutsideApplicationOnLockedScreenAsync();
        else if (deliveryMode == InformationDeliveryModes.VoiceAlert)
            await _voiceService.SpeakAsync(message);

        return false;
    }

    public async Task<bool> SendMessageAsync(DisplayAlertPage displayAlertPage)
    {
        await _popupNavigation.PushAsync(displayAlertPage, false);
        return await displayAlertPage.GetResultAsync();
    }

    public async Task<DisplayAlertPage> CreateDisplayAlertPageAsync(
        IList<ILink> message,
        string title = null,
        string cancel = null,
        string accept = null,
        bool isLazy = false)
    {
        var displayAlertViewModel = new DisplayAlertViewModel(message, title, cancel, accept);
        var displayAlertPage = new DisplayAlertPage(displayAlertViewModel, this);

        if (isLazy)
        {
            //This will be a relatively long job, so we need the current page to be displayed before we begin.
            //job more than 1s is long job
            await Task.Delay(500);
            //its dirty trick, but I have tried many ways to pre-render the page but none of them work.
            //If this is not done, then in a complexly structured page it will take a lot of time to display it.
            displayAlertPage.TranslationX = -8000;
            await _popupNavigation.PushAsync(displayAlertPage, false);
            await _popupNavigation.PopAsync();
            displayAlertPage.TranslationX = 0;
        }

        return displayAlertPage;
    }

    public async Task RemovePageAsync(PopupPage popupPage)
    {
        await _popupNavigation.RemovePageAsync(popupPage);
    }

    private void ShowOutsideApplicationAsync()
    {
        //todo: NotImplementedException
        throw new NotImplementedException();
    }

    private void ShowOutsideApplicationOnLockedScreenAsync()
    {
        throw new NotImplementedException();
    }
}
