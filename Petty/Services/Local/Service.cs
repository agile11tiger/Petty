namespace Petty.Services.Local
{
    public abstract class Service
    {
        protected Service()
        {
            _loggerService = MauiProgram.ServiceProvider.GetService<LoggerService>();
        }

        protected readonly LoggerService _loggerService;
    }
}
