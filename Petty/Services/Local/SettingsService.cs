namespace Petty.Services.Local
{
    public class SettingsService
    {
        public SettingsService()
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
