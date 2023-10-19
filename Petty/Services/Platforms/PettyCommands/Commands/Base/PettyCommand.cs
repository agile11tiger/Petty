using CommunityToolkit.Mvvm.Messaging;
using Petty.Services.Local.UserMessages;
using Petty.Services.Platforms.Paths;
namespace Petty.Services.Platforms.PettyCommands.Commands.Base;

public abstract class PettyCommand
{
    static PettyCommand()
    {
        _messager = MauiProgram.ServiceProvider.GetService<IMessenger>();
        _pathsService = MauiProgram.ServiceProvider.GetService<PathsService>();
        _loggerService = MauiProgram.ServiceProvider.GetService<LoggerService>();
        _audioPlayerService = MauiProgram.ServiceProvider.GetService<AudioPlayerService>();
        _localizationService = MauiProgram.ServiceProvider.GetService<LocalizationService>();
        _userMessagesService = MauiProgram.ServiceProvider.GetService<UserMessagesService>();
    }

    protected static readonly IMessenger _messager;
    protected static readonly PathsService _pathsService;
    protected static readonly LoggerService _loggerService;
    protected static readonly AudioPlayerService _audioPlayerService;
    protected static readonly LocalizationService _localizationService;
    protected static readonly UserMessagesService _userMessagesService;
}
