using Plugin.Maui.Audio;
namespace Petty.Services.Local;

public class AudioPlayerService(IAudioManager _audioManager) : Service
{
    public const string SCREENSHOT = "screenshot.mp3";

    public async Task PlayAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var fileStream = await FileSystem.OpenAppPackageFileAsync(fileName);
        var audioPlayer = _audioManager.CreateAsyncPlayer(fileStream);
        await audioPlayer.PlayAsync(cancellationToken);
    }
}
