namespace Petty.Services.Local
{
    public class SettingsService : Service
    {
        public SettingsService(LoggerService loggerService) : base(loggerService)
        {
            Settings = new Settings();
        }

        public Settings Settings { get; private set; }

        public void ApplySettings(Settings settings)
        {
            Settings = settings;
        }
    }
}
