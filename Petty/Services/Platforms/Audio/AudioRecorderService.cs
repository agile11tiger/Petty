namespace Petty.Services.Platforms.Audio
{
    /// <summary>
	/// A service that records audio on the device's microphone input.
	/// </summary> 
    /// <remarks>
    /// https://github.com/NateRickard/Plugin.AudioRecorder/blob/master/Plugin.AudioRecorder.Shared/AudioRecorderService.cs
    /// https://github.com/Dan0398/XamarinVoskSample
    /// </remarks>
    public partial class AudioRecorderService
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AudioRecorderService"/>.
        /// </summary>
        public AudioRecorderService(LoggerService loggerService, WaveRecorderService waveRecorderService)
        {
            Initialize();
            _loggerService = loggerService;
            _waveRecorderService = waveRecorderService;
        }

        private static bool _isStarting;
        private IAudioStream _audioStream;
        private readonly LoggerService _loggerService;
        private static readonly object _locker = new();
        private readonly List<float> _silenceThresholds = new();
        private readonly WaveRecorderService _waveRecorderService;

        public event Action<byte[]> BroadcastRecorderData;

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
        /// Gets/sets a value indicating the signal threshold that determines silence.
        /// If the recorder is being over or under aggressive when detecting silence, 
        /// you can alter this value to achieve different results.
        /// </summary>
        /// <remarks>Defaults to .15.  Value should be between 0 and 1.</remarks>
        public float SilenceThreshold { get; private set; } = .03f;

        /// <summary>
        /// Starts recording audio.
        /// </summary>
        public void StartRecording()
        {
            lock (_locker)
            {
                if (!_isStarting)
                {
                    _isStarting = true;
                    InitializeStream();
                    _audioStream.Start();
                }
            }
        }

        /// <summary>
        /// Stops recording audio.
        /// </summary>
        public void StopRecording()
        {
            lock (_locker)
            {
                if (BroadcastRecorderData.GetInvocationList().Length <= 1)
                {
                    _audioStream.Flush(); // allow the stream to send any remaining data
                    _audioStream.BroadcastData -= OnBroadcastDataAudioStream;
                    _audioStream.Stop();
                    _isStarting = false;
                }
            }
        }

        /// <summary>
        /// Starts recording audio to the specified path.
        /// </summary>
        public void StartRecording(string filePath)
        {
            //Todo: can this not working parallel with on some device???
            //recognizer always working
            InitializeStream();
            _waveRecorderService.StartRecorder(_audioStream, filePath);
        }

        /// <summary>
        /// Gets a new <see cref="Stream"/> to the recording audio file in readonly mode.
        /// </summary>
        /// <returns>A <see cref="Stream"/> object that can be used to read the audio file from the beginning.</returns>
        public Stream GetAudioFileStream()
        {
            return _waveRecorderService.GetAudioFileOnlyReadStream();
        }

        private void InitializeStream()
        {
            if (_audioStream == null)
            {
                lock (_locker)
                {
                    if (_audioStream != null)
                        return;

                    _audioStream = new AudioStream(_loggerService, PreferredSampleRate);
                    _audioStream.BroadcastData += OnBroadcastDataAudioStream;
                }
            }
        }

        private void OnBroadcastDataAudioStream(byte[] bytes)
        {
            var level = AudioFunctions.CalculateLevel(bytes);
            _silenceThresholds.Add(level);

            if (_silenceThresholds.Count > 180) //~1min
            {
                SilenceThreshold = _silenceThresholds.Average();
                _silenceThresholds.Clear();
            }

            if (level > SilenceThreshold) // skip silence bytes
                BroadcastRecorderData?.Invoke(bytes);
        }
    }
}
