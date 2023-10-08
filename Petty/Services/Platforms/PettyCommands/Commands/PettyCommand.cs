using Petty.Services.Platforms.Paths;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public abstract class PettyCommand
    {
        static PettyCommand()
        {
            _pathsService = MauiProgram.ServiceProvider.GetService<PathsService>();
            _voiceService = MauiProgram.ServiceProvider.GetService<VoiceService>();
            _loggerService = MauiProgram.ServiceProvider.GetService<LoggerService>();
            _audioPlayerService = MauiProgram.ServiceProvider.GetService<AudioPlayerService>();
            _localizationService = MauiProgram.ServiceProvider.GetService<LocalizationService>();
            _userMessagesService = MauiProgram.ServiceProvider.GetService<UserMessagesService>();
        }

        protected static readonly PathsService _pathsService;
        protected static readonly VoiceService _voiceService;
        protected static readonly LoggerService _loggerService;
        protected static readonly AudioPlayerService _audioPlayerService;
        protected static readonly LocalizationService _localizationService;
        protected static readonly UserMessagesService _userMessagesService;
    }
}
