using Petty.Resources.Localization;
using Petty.Services.Platforms.Paths;

namespace Petty.Services.Platforms.PettyCommands.Commands
{
    public class ScreenshotCommand : PettyCommand, IPettyCommand
    {
        public string Name => $"{AppResources.CommandPetName} {AppResources.CommandScreenshot}";

        public bool CheckComplianceCommand(string text)
        {
            return text.EndsWith(Name);
        }

        public async Task<bool> TryExecuteAsync()
        {
            try
            {
                if (!Screenshot.Default.IsCaptureSupported)
                    await _userMessagesService.SendMessageAsync(AppResources.UserMessageCommandNotSupported, AppResources.ButtonOk);

                var screenResult = await Screenshot.Default.CaptureAsync();
                var sourceStream = await screenResult.OpenReadAsync(ScreenshotFormat.Jpeg);
                var screenshotPath = System.IO.Path.Combine(PathsService.ScreenshotsPath, $"Screenshot_{DateTime.Now:yy.MM.dd_hh-mm-ss}.jpg");
                using FileStream localFileStream = System.IO.File.Open(screenshotPath, FileMode.OpenOrCreate);
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
}
