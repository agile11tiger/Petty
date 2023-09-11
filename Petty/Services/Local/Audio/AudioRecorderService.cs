using Petty.Platforms.Android.Audio;
using Petty.Services.Local.PettyCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Local.Audio
{
    /// <summary>
	/// A service that records audio on the device's microphone input.
	/// </summary> 
    /// <remarks>
    /// https://github.com/NateRickard/Plugin.AudioRecorder/blob/master/Plugin.AudioRecorder.Shared/AudioRecorderService.cs
    /// </remarks>
    public partial class AudioRecorderService
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AudioRecorderService"/>.
        /// </summary>
        public AudioRecorderService(PettyCommandsService pettyCommandsManager, LoggerService loggerService)
        {
            Initialize();
            _pettyCommandsManager = pettyCommandsManager;
            _logger = loggerService;
        }

        private const float NEAR_ZERO = .00000000001F;
        private const string DEFAULT_FILE_NAME = "record.wav";
        private bool _isAudioDetected;
        private DateTime? _startTime;
        private DateTime? _silenceTime;
        private WaveRecorder _recorder;
        private IAudioStream _audioStream;
        private readonly LoggerService _logger;
        private TaskCompletionSource<string> _recordTask;
        private readonly PettyCommandsService _pettyCommandsManager;


        /// <summary>
        /// Gets/sets the desired file path. If null it will be set automatically
        /// to a temporary file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets/sets the preferred sample rate to be used during recording.
        /// </summary>
        /// <remarks>This value may be overridden by platform-specific implementations, 
        /// e.g. the Android AudioManager will be asked for its preferred sample rate 
        /// and may override any user-set value here.
        /// </remarks>
        public int PreferredSampleRate { get; set; } = 44100;

        /// <summary>
        /// Returns a value indicating if the <see cref="AudioRecorderService"/> is currently recording audio
        /// </summary>
        public bool IsRecording => _audioStream?.Active ?? false;

        /// <summary>
        /// If <see cref="StopRecordingOnSilence"/> is set to <c>true</c>, this <see cref="TimeSpan"/> 
        /// indicates the amount of 'silent' time is required before recording is stopped.
        /// </summary>
        /// <remarks>Defaults to 2 seconds.</remarks>
        public TimeSpan AudioSilenceTimeout { get; set; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// If <see cref="StopRecordingAfterTimeout"/> is set to <c>true</c>, this <see cref="TimeSpan"/> 
        /// indicates the total amount of time to record audio for before recording is stopped. Defaults to 30 seconds.
        /// </summary>
        /// <seealso cref="StopRecordingAfterTimeout"/>
        public TimeSpan TotalAudioTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets/sets a value indicating if the <see cref="AudioRecorderService"/> should record without stopping.
        /// </summary>
        public bool IsTimelessRecord { get; set; }

        /// <summary>
        /// Gets/sets a value indicating if the <see cref="AudioRecorderService"/> should stop recording after silence (low audio signal) is detected.
        /// </summary>
        /// <remarks>Default is `false`</remarks>
        public bool StopRecordingOnSilence { get; set; } = false;

        /// <summary>
        /// Gets/sets a value indicating if the <see cref="AudioRecorderService"/> should stop recording after a certain amount of time.
        /// </summary>
        /// <remarks>Defaults to <c>false</c></remarks>
        /// <seealso cref="TotalAudioTimeout"/>
        public bool StopRecordingAfterTimeout { get; set; } = false;

        /// <summary>
        /// Gets/sets a value indicating the signal threshold that determines silence.
        /// If the recorder is being over or under aggressive when detecting silence, 
        /// you can alter this value to achieve different results.
        /// </summary>
        /// <remarks>Defaults to .15.  Value should be between 0 and 1.</remarks>
        public float SilenceThreshold { get; set; } = .15f;

        /// <summary>
        /// This event is raised when audio recording is complete and delivers a full filepath to the recorded audio file.
        /// </summary>
        /// <remarks> This event will be raised on a background thread to allow for any further processing needed.
        /// The audio file will be <c>null</c> in the case that no audio was recorded. </remarks>
        public event EventHandler<string> AudioInputReceived;

        /// <summary>
        /// Starts recording audio.
        /// </summary>
        /// <returns>A <see cref="Task"/> that will complete when recording is finished.  
        /// The task result will be the path to the recorded audio file, or null if no audio was recorded.</returns>
        public async Task<Task<string>> StartRecording()
        {
            FilePath ??= await GetDefaultFilePath();

            ResetAudioDetection();
            OnRecordingStarting();
            InitializeStream(PreferredSampleRate);
            await _recorder.StartRecorder(_audioStream, FilePath);

            _startTime = DateTime.Now;
            _recordTask = new TaskCompletionSource<string>();
            return _recordTask.Task;
        }

        /// <summary>
        /// Stops recording audio.
        /// </summary>
        /// <param name="continueProcessing"><c>true</c> (default) to finish recording and raise the <see cref="AudioInputReceived"/> event. 
        /// Use <c>false</c> here to stop recording but do nothing further (from an error state, etc.).</param>
        public async Task StopRecording(bool continueProcessing = true)
        {
            _audioStream.Flush(); // allow the stream to send any remaining data
            _audioStream.OnBroadcast -= AudioStream_OnBroadcast;

            try
            {
                await _audioStream.Stop();
                // WaveRecorder will be stopped as result of stream stopping
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }

            OnRecordingStopped();

            var returnedFilePath = GetAudioFilePath();
            // complete the recording Task for anything waiting on this
            _recordTask.TrySetResult(returnedFilePath);

            if (continueProcessing)
                AudioInputReceived?.Invoke(this, returnedFilePath);
        }

        /// <summary>
        /// Gets the full filepath to the recorded audio file.
        /// </summary>
        /// <returns>The full filepath to the recorded audio file, or null if no audio was detected during the last record.</returns>
        public string GetAudioFilePath()
        {
            return _isAudioDetected ? FilePath : null;
        }

        /// <summary>
        /// Gets a new <see cref="Stream"/> to the recording audio file in readonly mode.
        /// </summary>
        /// <returns>A <see cref="Stream"/> object that can be used to read the audio file from the beginning.</returns>
        public Stream GetAudioFileStream()
        {
            return _recorder.GetAudioFileOnlyReadStream();
        }

        private void InitializeStream(int sampleRate)
        {
            try
            {
                if (_audioStream != null)
                    _audioStream.OnBroadcast -= AudioStream_OnBroadcast;
                else
                    _audioStream = new AudioStream(_logger, sampleRate);

                _audioStream.OnBroadcast += AudioStream_OnBroadcast;
                _recorder ??= new WaveRecorder(_logger);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }
        }

        private void ResetAudioDetection()
        {
            _isAudioDetected = false;
            _silenceTime = null;
            _startTime = null;
        }

        private void AudioStream_OnBroadcast(object sender, byte[] bytes)
        {
            //if (IsTimelessRecord)
                _pettyCommandsManager.ProcessRawData(bytes);
            //else
            //{
                var level = AudioFunctions.CalculateLevel(bytes);

                if (level < NEAR_ZERO && !_isAudioDetected) // discard any initial 0s so we don't jump the gun on timing out
                {
                    _logger.Log($"level == {level} && {!_isAudioDetected}");
                    return;
                }

                if (level > SilenceThreshold) // did we find a signal?
                {
                    _isAudioDetected = true;
                    _silenceTime = null;
                
                    _logger.Log($"{DateTime.Now}. level > SilenceThreshold; bytes: {bytes.Length}; level: {level}");
                }
                else // no audio detected
                {
                    // see if we've detected 'near' silence for more than <audioTimeout>
                    if (StopRecordingOnSilence && _silenceTime.HasValue)
                    {
                        var currentTime = DateTime.Now;

                        if (currentTime.Subtract(_silenceTime.Value).TotalMilliseconds > AudioSilenceTimeout.TotalMilliseconds)
                        {
                            Timeout($"AudioStream_OnBroadcast :: {currentTime} :: AudioSilenceTimeout exceeded, stopping recording :: Near-silence detected at: {_silenceTime}");
                            return;
                        }
                    }
                    else
                    {
                        _silenceTime = DateTime.Now;
                        _logger.Log($"{_silenceTime}. Near-silence detected. bytes: {bytes.Length}; level: {level}");
                    }
                }

                if (StopRecordingAfterTimeout && DateTime.Now - _startTime > TotalAudioTimeout)
                    Timeout("AudioStream_OnBroadcast(): TotalAudioTimeout exceeded, stopping recording");
            //}
        }

        void Timeout(string reason)
        {
            Debug.WriteLine(reason);
            _audioStream.OnBroadcast -= AudioStream_OnBroadcast; // need this to be immediate or we can try to stop more than once

            // since we're in the middle of handling a broadcast event when an audio timeout occurs, we need to break the StopRecording call on another thread
            //	Otherwise, Bad. Things. Happen.
            _ = Task.Run(() => StopRecording());
        }
    }
}
