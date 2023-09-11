using Android.Media;
using Android.Util;
using Petty.Services.Local.Audio;
using Petty.Services.Local.PermissionsFolder;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Petty.Platforms.Android.Audio
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
            LoggerService logger,
            int sampleRate = 44100, 
            ChannelIn channels = ChannelIn.Mono, 
            Encoding audioFormat = Encoding.Pcm16bit)
        {
            _bufferSize = AudioRecord.GetMinBufferSize(sampleRate, channels, audioFormat) * 8;

            //TODO: try catch
            if (_bufferSize < 0)
                throw new Exception("Invalid buffer size calculated; audio settings used may not be supported on this device");

            _logger = logger;
            SampleRate = sampleRate;
            _channels = channels;
            _audioFormat = audioFormat;

            //var packageManager = global::Android.App.Application.Context.PackageManager;
            //var hasMicrophone = packageManager.HasSystemFeature(global::Android.Content.PM.PackageManager.FeatureMicrophone);
        }

        private readonly int _bufferSize;
        private readonly LoggerService _logger;
        private readonly ChannelIn _channels = ChannelIn.Mono;
        private readonly Encoding _audioFormat = Encoding.Pcm16bit;

        /// <summary>
        /// The audio record.
        /// </summary>
        private AudioRecord _audioRecord;

        /// <summary>
        /// Occurs when new audio has been streamed.
        /// </summary>
        public event EventHandler<byte[]> OnBroadcast;

        /// <summary>
        /// Occurs when the audio stream active status changes.
        /// </summary>
        public event EventHandler<bool> OnActiveChanged;

        /// <summary>
        /// Occurs when there's an error while capturing audio.
        /// </summary>
        public event EventHandler<Exception> OnException;

        /// <summary>
        /// The default device.
        /// </summary>
        public const AudioSource AUDIO_SOURCE_DEFAULT = AudioSource.Mic;

        /// <summary>
        /// Gets the sample rate.
        /// </summary>
        public int SampleRate { get; private set; } = 44100;

        /// <summary>
        /// Gets bits per sample.
        /// </summary>
        public int BitsPerSample => (_audioRecord.AudioFormat == Encoding.Pcm16bit) ? 16 : 8;

        /// <summary>
        /// Gets the channel count.
        /// </summary>      
        public int ChannelCount => _audioRecord.ChannelCount;

        /// <summary>
        /// Gets the average data transfer rate in bytes per second.
        /// </summary>
        public int AverageBytesPerSecond => SampleRate * BitsPerSample / 8 * ChannelCount;

        /// <summary>
        /// Gets a value indicating if the audio stream is active.
        /// </summary>
        public bool Active => _audioRecord?.RecordingState == RecordState.Recording;

        private void Init()
        {
            Stop(); // just in case

            if (MicrophonePermission.GetAsync().Result == PermissionStatus.Granted)
                _audioRecord = new AudioRecord(AUDIO_SOURCE_DEFAULT, SampleRate ,_channels,_audioFormat, _bufferSize);

            if (_audioRecord.State == State.Uninitialized)
                throw new Exception("Unable to successfully initialize AudioStream; reporting State.Uninitialized. " +
                    " If using an emulator, make sure it has access to the system microphone.");
        }

        /// <summary>
        /// Starts the audio stream.
        /// </summary>
        public Task Start()
        {
            try
            {
                if (!Active)
                {
                    // not sure this does anything or if should be here... inherited via copied code ¯\_(ツ)_/¯
                    global::Android.OS.Process.SetThreadPriority(global::Android.OS.ThreadPriority.UrgentAudio);
                    Init();
                    _audioRecord.StartRecording();
                    OnActiveChanged?.Invoke(this, true);
                    Task.Run(() => Record());
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                Stop();
                throw;
            }
        }

        /// <summary>
        /// Stops the audio stream.
        /// </summary>
        public Task Stop()
        {
            if (Active)
            {
                _audioRecord.Stop();
                _audioRecord.Release();
                OnActiveChanged?.Invoke(this, false);
            }
            else // just in case
                _audioRecord?.Release();

            return Task.FromResult(true);
        }

        /// <summary>
        /// Record from the microphone and broadcast the buffer.
        /// </summary>
        private async Task Record()
        {
            var data = new byte[_bufferSize];
            var readFailureCount = 0;
            var readResult = 0;

            while (Active)
            {
                try
                {
                    // not sure if this is even a good idea, but we'll try to allow a single bad read, and past that shut it down
                    if (readFailureCount > 1)
                    {
                        _logger.Log("AudioStream.Record(): Multiple read failures detected, stopping stream");
                        await Stop();
                        break;
                    }

                    readResult = _audioRecord.Read(data, 0, _bufferSize); // this can block if there are no bytes to read

                    // readResult should == the # bytes read, except a few special cases
                    if (readResult > 0)
                    {
                        readFailureCount = 0;
                        OnBroadcast?.Invoke(this, data);
                    }
                    else
                    {
                        _logger.Log($"AudioStream.Record(): readResult returned error code: {readResult}");

                        switch (readResult)
                        {
                            case (int)TrackStatus.ErrorInvalidOperation:
                            case (int)TrackStatus.ErrorBadValue:
                            case (int)TrackStatus.ErrorDeadObject:
                                await Stop();
                                break;
                            //case (int)TrackStatus.Error:
                            default:
                                readFailureCount++;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(ex);
                    readFailureCount++;
                    OnException?.Invoke(this, ex);
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
