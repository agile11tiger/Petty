namespace Petty.Services.Platforms.Audio
{
    public interface IAudioStream
    {
        /// <summary>
        /// Occurs when new audio has been streamed.
        /// </summary>
        event Action<byte[]> BroadcastData;

        /// <summary>
        /// Occurs when there's an error while capturing audio.
        /// </summary>
        event Action<Exception> ExceptionCatched;

        /// <summary>
        /// Gets a value indicating if the audio stream is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Gets the sample rate.
        /// </summary>
        /// <value>
        /// The sample rate.
        /// </value>
        int SampleRate { get; }

        /// <summary>
        /// Gets the channel count.
        /// </summary>
        /// <value>
        /// The channel count.
        /// </value>
        int ChannelCount { get; }

        /// <summary>
        /// Gets bits per sample.
        /// </summary>
        int BitsPerSample { get; }

        /// <summary>
        /// Starts the audio stream.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the audio stream.
        /// </summary>
        void Stop();

        /// <summary>
        /// Flushes any audio bytes in memory but not yet broadcast out to any listeners.
        /// </summary>
        void Flush();
    }
}
