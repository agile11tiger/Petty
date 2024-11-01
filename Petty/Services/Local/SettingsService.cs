﻿namespace Petty.Services.Local;

public class SettingsService : Service
{
    public SettingsService(DatabaseService databaseService)
    {
        Settings = databaseService.ApplicationDbContext.Settings.FirstOrDefault();

        if (Settings == default)
        {
            var settings = new Settings() { BaseSettings = new(), VoiceSettings = new() };
            databaseService.CreateOrUpdateAsync(settings).Wait();
            Settings = settings;
        }
    }

    public Settings Settings { get; private set; }

    public void ApplySettings(Settings settings)
    {
        Settings = settings;
    }
}
