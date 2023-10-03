namespace Petty.Services.Local
{
    public class DialogService : Service
    {
        public DialogService(LoggerService loggerService)
            : base(loggerService)
        {
        }

        public Task ShowAlertAsync(string message, string title, string buttonLabel)
        {
            throw new NotImplementedException();
        }
    }
}
