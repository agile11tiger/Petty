namespace Petty.Services.Local.UserMessages
{
    public class UserMessagesService : Service
    {
        public UserMessagesService(
            VoiceService voiceService,
            LoggerService loggerService,
            SettingsService settingsService)
            : base(loggerService)
        {
            _voiceService = voiceService;
            _settingsService = settingsService;
        }

        private readonly VoiceService _voiceService;
        private readonly SettingsService _settingsService;

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
                var isAnswerRecevied = false;

                if (!Thread.CurrentThread.IsBackground)
                    answer = await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
                else
                {
                    Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                    {
                        answer = await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
                        isAnswerRecevied = true;
                    });

                    while (!isAnswerRecevied)
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
