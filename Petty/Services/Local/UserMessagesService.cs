namespace Petty.Services.Local
{
    public class UserMessagesService
    {
        public UserMessagesService(LoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        private readonly LoggerService _loggerService;

        public async Task SendMessageAsync(string message, string cancel, string accept = "", string title = "")
        {
            await SendRequestAsync(message, cancel, accept, title);
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
                    App.Current.MainPage.Dispatcher.Dispatch(async () =>
                    {
                        answer = await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
                        isAnswerRecevied = true;
                    });

                while (!isAnswerRecevied)
                    await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
            }

            return answer;
        }
    }
}
