using CommunityToolkit.Mvvm.Messaging;
using Petty.Resources.Localization;
using Petty.Services.Platforms.PettyCommands.Commands.Base;

namespace Petty.Services.Platforms.PettyCommands.Commands.Camera
{
    public class PickVideoFilesCommand : PettyCommand, IPettyCommand
    {
        private readonly PickOptions _pickOptions = new() { FileTypes = FilePickerFileType.Videos };
        public string Name => AppResources.CommandPickVideoFiles;
        public string Description => AppResources.CommandPickVideoFilesDescription;

        public async Task<bool> TryExecuteAsync()
        {
            try
            {
                var videoFiles = await FilePicker.Default.PickMultipleAsync(_pickOptions);

                if (videoFiles != null)
                {
                    _messager.Send(videoFiles);
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
