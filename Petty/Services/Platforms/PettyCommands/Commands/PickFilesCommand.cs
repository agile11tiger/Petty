using CommunityToolkit.Mvvm.Messaging;
using Petty.Resources.Localization;
using Petty.Services.Platforms.Paths;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public class PickFilesCommand : PettyCommand, IPettyCommand
    {
        public string Name => AppResources.CommandPickFiles;
        public string Description => AppResources.CommandPickFilesDescription;

        public async Task<bool> TryExecuteAsync()
        {
            try
            {
                var files = await FilePicker.Default.PickMultipleAsync();

                if (files.Any())
                {
                    _messager.Send(files);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
            }

            return false;
        }
    }
}
