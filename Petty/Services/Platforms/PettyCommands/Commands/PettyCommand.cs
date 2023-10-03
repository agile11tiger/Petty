using Petty.Services.Platforms.Paths;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public abstract class PettyCommand
    {
        public PettyCommand()
        {
            _pathsService = MauiProgram.ServiceProvider.GetService<PathsService>();
            _loggerService = MauiProgram.ServiceProvider.GetService<LoggerService>();
            _audioPlayerService = MauiProgram.ServiceProvider.GetService<AudioPlayerService>();
            _userMessagesService = MauiProgram.ServiceProvider.GetService<UserMessagesService>();
        }

        protected readonly PathsService _pathsService;
        protected readonly LoggerService _loggerService;
        protected readonly AudioPlayerService _audioPlayerService;
        protected readonly UserMessagesService _userMessagesService;
    }
}
