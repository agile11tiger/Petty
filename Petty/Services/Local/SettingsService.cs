namespace Petty.Services.Local
{
    public class SettingsService : Service
    {
        public SettingsService(LoggerService loggerService, ApplicationDbContext applicationDbContext, DatabaseService databaseService) : base(loggerService)
        {
            Settings = databaseService.ApplicationDbContext.Settings.FirstOrDefault();

            if (Settings == default)
            {
                try
                {
                    var settings = new Settings() { BaseSettings = new(), VoiceSettings = new() };
                    databaseService.CreateOrUpdateAsync(settings).Wait();
                    Settings = settings;
                }
                catch (Exception ex)
                {
                    loggerService.Log(ex);
                }
            }
        }

        public Settings Settings { get; private set; }

        public void ApplySettings(Settings settings)
        {
            Settings = settings;
        }
    }
}
