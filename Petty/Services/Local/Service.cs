namespace Petty.Services.Local
{
    public abstract class Service
    {
        protected Service(LoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        protected readonly LoggerService _loggerService;
    }
}
