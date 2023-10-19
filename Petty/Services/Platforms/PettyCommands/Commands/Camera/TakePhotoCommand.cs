using CommunityToolkit.Mvvm.Messaging;
using Petty.Resources.Localization;
using Petty.Services.Platforms.Paths;
using Petty.Services.Platforms.PettyCommands.Commands.Base;
namespace Petty.Services.Platforms.PettyCommands.Commands.Camera;

public class TakePhotoCommand : PettyCommand, IPettyCommand
{
    public string Name => AppResources.CommandTakePhoto;
    public string Description => AppResources.CommandTakeVideo;

    public async Task<bool> TryExecuteAsync()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
                await _userMessagesService.SendMessageAsync(AppResources.UserMessageCommandNotSupported);

            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                var picturePath = Path.Combine(PathsService.PicturesPath, $"Picture_{DateTime.Now:yy.MM.dd_hh-mm-ss}.jpg");
                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(picturePath);
                await sourceStream.CopyToAsync(localFileStream);
                _messager.Send(new FileResult(picturePath));
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
