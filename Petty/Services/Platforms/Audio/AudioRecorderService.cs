namespace Petty.Services.Platforms.Audio;

/// <summary>
/// A service that records audio on the device's microphone input.
/// </summary> 
/// <remarks>
/// https://github.com/NateRickard/Plugin.AudioRecorder/blob/master/Plugin.AudioRecorder.Shared/AudioRecorderService.cs
/// https://github.com/Dan0398/XamarinVoskSample
/// </remarks>
public partial class AudioRecorderService : Service
{
    /// <summary>
    /// Creates a new instance of the <see cref="AudioRecorderService"/>.
    /// </summary>
    public AudioRecorderService(IAudioStream audioStream, LoggerService loggerService, WaveRecorderService waveRecorderService)
    {
        Initialize();
        _audioStream = audioStream;
        _waveRecorderService = waveRecorderService;
    }

    private readonly IAudioStream _audioStream;
    private readonly static object _locker = new();
    private readonly WaveRecorderService _waveRecorderService;

    private event Action<byte[]> BroadcastRecorderData;

    /// <summary>
    /// Gets/sets the preferred sample rate to be used during recording.
    /// </summary>
    /// <remarks>This value may be overridden by platform-specific implementations, 
    /// e.g. the Android AudioManager will be asked for its preferred sample rate 
    /// and may override any user-set value here.
    /// Not sure, but i think vosk model was trained on this frequency: 16000.
    /// </remarks>
    public int PreferredSampleRate { get; set; } = 16000;

    /// <summary>
    /// Returns a value indicating if the <see cref="AudioRecorderService"/> is currently recording audio
    /// </summary>
    public bool IsRecording => _audioStream?.IsActive ?? false;

    /// <summary>
    /// Starts recording audio.
    /// </summary>
    public void StartRecording(Action<byte[]> subscriber)
    {
        lock (_locker)
        {
            if (BroadcastRecorderData == null)
                _audioStream.Start(OnBroadcastDataAudioStream);

            BroadcastRecorderData += subscriber;
        }
    }

    /// <summary>
    /// Stops recording audio.
    /// </summary>
    public void StopRecording(Action<byte[]> subscriber)
    {
        lock (_locker)
        {
            if (BroadcastRecorderData.GetInvocationList().Length == 1)
            {
                _audioStream.Flush(); // allow the stream to send any remaining data
                _audioStream.Stop(OnBroadcastDataAudioStream);
            }

            BroadcastRecorderData -= subscriber;
        }
    }

    /// <summary>
    /// Starts recording audio to the specified path.
    /// </summary>
    public void StartRecording(string filePath)
    {
        lock (_locker)
        {
            //Todo: can this not working parallel with on some device???
            //recognizer always working
            _waveRecorderService.StartRecorder(_audioStream, filePath);
        }
    }

    /// <summary>
    /// Starts recording audio to the specified path.
    /// </summary>
    public void StopRecording(string filePath, Action<byte[]> subscriber)
    {
        lock (_locker)
        {
            _waveRecorderService.StopRecorder();
        }
    }

    /// <summary>
    /// Gets a new <see cref="Stream"/> to the recording audio file in readonly mode.
    /// </summary>
    /// <returns>A <see cref="Stream"/> object that can be used to read the audio file from the beginning.</returns>
    public Stream GetAudioFileStream()
    {
        return _waveRecorderService.GetAudioFileOnlyReadStream();
    }

    private void OnBroadcastDataAudioStream(byte[] bytes)
    {
        BroadcastRecorderData?.Invoke(bytes);
    }
}
