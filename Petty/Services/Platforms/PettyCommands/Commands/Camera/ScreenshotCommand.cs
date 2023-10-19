using Petty.Resources.Localization;
using Petty.Services.Platforms.Paths;
using Petty.Services.Platforms.PettyCommands.Commands.Base;
namespace Petty.Services.Platforms.PettyCommands.Commands.Camera;

public class ScreenshotCommand : PettyCommand, IPettyCommand
{
    public string Name => AppResources.CommandScreenshot;
    public string Description => AppResources.CommandScreenshotDescription;

    public async Task<bool> TryExecuteAsync()
    {
        try
        {
            if (!Screenshot.Default.IsCaptureSupported)
                await _userMessagesService.SendMessageAsync(AppResources.UserMessageCommandNotSupported);

            var screenResult = await Screenshot.Default.CaptureAsync();
            var sourceStream = await screenResult.OpenReadAsync(ScreenshotFormat.Jpeg);
            var screenshotPath = Path.Combine(PathsService.ScreenshotsPath, $"Screenshot_{DateTime.Now:yy.MM.dd_hh-mm-ss}.jpg");
            using FileStream localFileStream = File.Open(screenshotPath, FileMode.OpenOrCreate);
            await sourceStream.CopyToAsync(localFileStream);
            await _audioPlayerService.PlayAsync(AudioPlayerService.SCREENSHOT);
            return true;
        }
        catch (Exception ex)
        {
            _loggerService.Log(ex);
        }

        return false;
    }
}
