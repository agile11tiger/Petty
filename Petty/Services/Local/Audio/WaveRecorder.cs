using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Local.Audio
{
    internal class WaveRecorder : IDisposable
    {
        public WaveRecorder(LoggerService logger)
        {
            _logger = logger;
        }

        private int _byteCount;
        private BinaryWriter _writer;
        private string _audioFilePath;
        private FileStream _fileStream;
        private IAudioStream _audioStream;
        private StreamWriter _streamWriter;
        private readonly LoggerService _logger;

        /// <summary>
        /// Starts recording WAVE format audio from the audio stream.
        /// </summary>
        /// <param name="stream">A <see cref="IAudioStream"/> that provides the audio data.</param>
        /// <param name="filePath">The full path of the file to record audio to.</param>
        public async Task StartRecorder(IAudioStream stream, string filePath)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            try
            {
                //if we're restarting, let's see if we have an existing stream configured that can be stopped
                if (_audioStream != null)
                    await _audioStream.Stop();

                _byteCount = 0;
                _audioStream = stream;
                _audioFilePath = filePath;
                _fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                _streamWriter = new StreamWriter(_fileStream);
                _writer = new BinaryWriter(_streamWriter.BaseStream, Encoding.UTF8);
                _audioStream.OnBroadcast += OnStreamBroadcast;
                _audioStream.OnActiveChanged += StreamActiveChanged;

                if (!_audioStream.Active)
                    await _audioStream.Start();
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                StopRecorder();
                throw;
            }
        }

        /// <summary>
        /// Gets a new <see cref="Stream"/> to the audio file in readonly mode.
        /// </summary>
        /// <returns>A <see cref="Stream"/> object that can be used to read the audio file from the beginning.</returns>
        public Stream GetAudioFileOnlyReadStream()
        {
            //return a new stream to the same audio file, in Read mode
            return new FileStream(_audioFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        /// <summary>
        /// Stops recording WAV audio from the underlying <see cref="IAudioStream"/> and finishes writing the WAV file.
        /// </summary>
        public void StopRecorder()
        {
            try
            {
                if (_audioStream != null)
                {
                    _audioStream.OnBroadcast -= OnStreamBroadcast;
                    _audioStream.OnActiveChanged -= StreamActiveChanged;
                }

                if (_writer != null)
                {
                    if (_streamWriter.BaseStream.CanWrite)
                    {
                        //now that audio is finished recording, write a WAV/RIFF header at the beginning of the file
                        _writer.Seek(0, SeekOrigin.Begin);
                        AudioFunctions.WriteWavHeader(_writer, _audioStream.ChannelCount, _audioStream.SampleRate, _audioStream.BitsPerSample, _byteCount);
                    }

                    _writer.Dispose(); //this should properly close/dispose the underlying stream as well
                    _writer = null;
                    _fileStream = null;
                    _streamWriter = null;
                }

                _audioStream = null;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                throw;
            }
        }

        public void Dispose()
        {
            StopRecorder();
        }

        private void StreamActiveChanged(object sender, bool active)
        {
            if (!active)
                StopRecorder();
        }

        private void OnStreamBroadcast(object sender, byte[] bytes)
        {
            try
            {
                if (_writer != null && _streamWriter != null)
                {
                    _writer.Write(bytes);
                    _byteCount += bytes.Length;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                StopRecorder();
            }
        }
    }
}
