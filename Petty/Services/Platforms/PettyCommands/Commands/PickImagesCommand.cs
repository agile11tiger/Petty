using CommunityToolkit.Mvvm.Messaging;
using Petty.Resources.Localization;
using Petty.Services.Platforms.Paths;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public class PickImagesCommand : PettyCommand, IPettyCommand
    {
        public string Name => AppResources.CommandPickImages;
        public string Description => AppResources.CommandPickImagesDescription;

        public async Task<bool> TryExecuteAsync()
        {
            try
            {
                var images = await FilePicker.Default.PickMultipleAsync(PickOptions.Images);

                if (images != null)
                {
                    _messager.Send(images);
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
