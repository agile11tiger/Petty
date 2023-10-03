using Android.Media;
using Petty.Services.Local.PermissionsFolder;

namespace Petty.Services.Platforms.Audio
{
    internal class AudioStream : IAudioStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioStream"/> class.
        /// </summary>
        /// <param name="sampleRate">Sample rate.</param>
        /// <param name="channels">The <see cref="ChannelIn"/> value representing the number of channels to record.</param>
        /// <param name="audioFormat">The format of the recorded audio.</param>
        public AudioStream(
            LoggerService loggerService,
            int sampleRate = 16000,
            ChannelIn channels = ChannelIn.Mono,
            Encoding audioFormat = Encoding.Pcm16bit,
            AudioSource audioSource = AudioSource.Mic)
        {
            _bufferSize = AudioRecord.GetMinBufferSize(sampleRate, channels, audioFormat) * 8;

            if (_bufferSize < 0)
                throw new Exception("Invalid buffer size calculated; audio settings used may not be supported on this device");

            _channels = channels;
            _sampleRate = sampleRate;
            _audioFormat = audioFormat;
            _audioSource = audioSource;
            _loggerService = loggerService;

            //todo: Добавить в диагностику
            //var packageManager = global::Android.App.Application.Context.PackageManager;
            //var hasMicrophone = packageManager.HasSystemFeature(global::Android.Content.PM.PackageManager.FeatureMicrophone);
        }

        private AudioRecord _audioRecord;
        private readonly int _bufferSize;
        private readonly int _sampleRate;
        private Thread _audioStreamThread;
        private readonly ChannelIn _channels;
        private readonly Encoding _audioFormat;
        private readonly AudioSource _audioSource;
        private readonly LoggerService _loggerService;
        private static readonly object _locker = new();

        private event Action<byte[]> BroadcastData;
        public event Action<bool> ActiveStatusChanged;
        public event Action<Exception> ExceptionCatched;

        public int ChannelCount => _audioRecord.ChannelCount;
        public int SampleRate { get => _audioRecord.SampleRate; }
        public bool IsActive => _audioRecord?.RecordingState == RecordState.Recording;
        public int BitsPerSample => (_audioRecord.AudioFormat == Encoding.Pcm16bit) ? 16 : 8;
        public int AverageBytesPerSecond => _sampleRate * BitsPerSample / 8 * ChannelCount;

        public void Start(Action<byte[]> subscriber)
        {
            lock (_locker)
            {
                try
                {
                    if (!IsActive)
                    {
                        // not sure this does anything or if should be here... inherited via copied code ¯\_(ツ)_/¯
                        global::Android.OS.Process.SetThreadPriority(global::Android.OS.ThreadPriority.UrgentAudio);
                        Init();
                        _audioRecord.StartRecording();
                        _audioStreamThread = new Thread(Record)
                        {
                            IsBackground = true,
                            Priority = ThreadPriority.AboveNormal,
                            Name = nameof(AudioStream) + "Thread"
                        };
                        _audioStreamThread.Start();
                    }

                    BroadcastData += subscriber;
                }
                catch (Exception ex)
                {
                    _loggerService.Log(ex);
                    throw;
                }
            }
        }

        public void Stop(Action<byte[]> subscriber)
        {
            lock (_locker)
            {
                if (IsActive && BroadcastData.GetInvocationList().Length == 1)
                {
                    _audioRecord.Stop();
                    _audioStreamThread = null;
                }

                _audioRecord.Release();
                BroadcastData -= subscriber;
            }
        }

        private void Init()
        {
            if (MicrophonePermission.GetAsync().Result == PermissionStatus.Granted)
                _audioRecord = new AudioRecord(_audioSource, _sampleRate, _channels, _audioFormat, _bufferSize);

            if (_audioRecord.State == State.Uninitialized)
                throw new Exception("Unable to successfully initialize AudioStream; reporting State.Uninitialized. " +
                    " If using an emulator, make sure it has access to the system microphone.");
        }

        /// <summary>
        /// Record from the microphone and broadcast the buffer.
        /// </summary>
        private void Record()
        {
            var readResult = 0;
            var readFailureCount = 0;
            var data = new byte[_bufferSize];

            while (IsActive)
            {
                try
                {
                    // not sure if this is even a good idea, but we'll try to allow a single bad read, and past that shut it down
                    if (readFailureCount > 1)
                        throw new ThreadStateException();

                    readResult = _audioRecord.Read(data, 0, _bufferSize); // this can block if there are no bytes to read

                    // readResult should == the # bytes read, except a few special cases
                    if (readResult > 0)
                    {
                        readFailureCount = 0;
                        BroadcastData?.Invoke(data);
                    }
                    else
                    {
                        _loggerService.Log($"AudioStream.Record(): readResult returned error code: {readResult}");

                        switch (readResult)
                        {
                            case (int)TrackStatus.ErrorInvalidOperation:
                            case (int)TrackStatus.ErrorBadValue:
                            case (int)TrackStatus.ErrorDeadObject:
                                throw new ThreadStateException($"readResult: {readResult}");
                            //case (int)TrackStatus.Error:
                            default:
                                readFailureCount++;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    readFailureCount++;
                    _loggerService.Log($"ReadFailureCount: {readFailureCount}", ex);
                    ExceptionCatched?.Invoke(ex);
                }
            }
        }

        /// <summary>
        /// Flushes any audio bytes in memory but not yet broadcast out to any listeners.
        /// </summary>
        public void Flush()
        {
            // not needed for this implementation
        }
    }
}
