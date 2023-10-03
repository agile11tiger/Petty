using Petty.Services.Platforms.PettyCommands.Commands;

namespace Petty.Services.Local
{
    public class PettyVoiceService : Service
    {
        public PettyVoiceService(LoggerService loggerService) : base(loggerService)
        {
        }

        public void PlayCommandExecutionFailed(IPettyCommand command)
        {

        }

        public void PlayCommandExecutionSuccessed(IPettyCommand command)
        {

        }
    }
}
