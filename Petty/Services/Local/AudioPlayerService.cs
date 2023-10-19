using Plugin.Maui.Audio;
namespace Petty.Services.Local;

public class AudioPlayerService
{
    public AudioPlayerService(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public const string SCREENSHOT = "screenshot.mp3";

    private readonly IAudioManager _audioManager;

    public async Task PlayAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var fileStream = await FileSystem.OpenAppPackageFileAsync(fileName);
        var audioPlayer = _audioManager.CreateAsyncPlayer(fileStream);
        await audioPlayer.PlayAsync(cancellationToken);
    }
}
