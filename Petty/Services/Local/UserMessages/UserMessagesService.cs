using Mopups.Interfaces;
using Petty.ViewModels.Components.DisplayAlert;
using Petty.Views.Components;

namespace Petty.Services.Local.UserMessages
{
    public class UserMessagesService : Service
    {
        public UserMessagesService(
            VoiceService voiceService,
            LoggerService loggerService,
            IPopupNavigation popupNavigation)
            : base(loggerService)
        {
            _voiceService = voiceService;
            _popupNavigation = popupNavigation;
        }

        private readonly VoiceService _voiceService;
        private readonly IPopupNavigation _popupNavigation;

        public async Task SendMessageAsync(DisplayAlertPage displayAlertPage)
        {
            await _popupNavigation.PushAsync(displayAlertPage);
            //await Application.Current.MainPage.DisplayAlert(null, "Lol\r\nLolWithLink", "accept", "cancel");
        }

        public async Task<DisplayAlertPage> CreateDisplayAlertPage(
            IList<ILink> message,
            string cancel = null,
            string title = null,
            bool isLazy = false,
            string accept = null)
        {
            var displayAlertViewModel = new DisplayAlertViewModel(message, cancel, title, accept);
            var displayAlertPage = new DisplayAlertPage(displayAlertViewModel);

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

        public async Task SendMessageAsync(
            string message,
            string cancel,
            InformationDeliveryModes deliveryMode = InformationDeliveryModes.DisplayAlertInApp,
            string accept = "",
            string title = "")
        {
            if (deliveryMode == InformationDeliveryModes.DisplayAlertInApp)
                await SendRequestAsync(message, cancel, accept, title);
            else if (deliveryMode == InformationDeliveryModes.DisplayAlertOutsideApp)
                ShowOutsideApplication();
            else if (deliveryMode == InformationDeliveryModes.DisplayAlertOutsideAppOnLockedScreen)
                ShowOutsideApplicationOnLockedScreen();
            else if (deliveryMode == InformationDeliveryModes.VoiceAlert)
                await _voiceService.SpeakAsync(message);
        }

        public async Task<bool> SendRequestAsync(string message, string cancel, string accept = "", string title = "")
        {
            var answer = false;

            try
            {
                var isAnswerReceived = false;

                if (!Thread.CurrentThread.IsBackground)
                    answer = await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
                else
                {
                    Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                    {
                        //todo: need custom display alert with the addition of a "more details" button.
                        answer = await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
                        isAnswerReceived = true;
                    });

                    while (!isAnswerReceived)
                        await Task.Delay(500);
                }

            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
            }

            return answer;
        }

        public void ShowOutsideApplication()
        {
            //todo: NotImplementedException
            throw new NotImplementedException();
        }

        public void ShowOutsideApplicationOnLockedScreen()
        {
            throw new NotImplementedException();
        }

    }
}
