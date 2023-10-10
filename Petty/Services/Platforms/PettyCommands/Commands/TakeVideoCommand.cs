using CommunityToolkit.Mvvm.Messaging;
using Petty.Resources.Localization;
using Petty.Services.Platforms.Paths;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public class TakeVideoCommand : PettyCommand, IPettyCommand
    {
        public string Name => AppResources.CommandTakeVideo;
        public string Description => AppResources.CommandTakeVideoDescription;

        public async Task<bool> TryExecuteAsync()
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                    await _userMessagesService.SendMessageAsync(AppResources.UserMessageCommandNotSupported, AppResources.ButtonOk);

                var video = await MediaPicker.Default.CaptureVideoAsync();

                if (video != null)
                {
                    var videoPath = System.IO.Path.Combine(PathsService.VideoPath, $"Video_{DateTime.Now:yy.MM.dd_hh-mm-ss}.jpg");
                    using Stream sourceStream = await video.OpenReadAsync();
                    using FileStream localFileStream = File.OpenWrite(videoPath);
                    await sourceStream.CopyToAsync(localFileStream);
                    _messager.Send(new FileResult(videoPath));
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
