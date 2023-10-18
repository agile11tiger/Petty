using System.Text;

namespace Petty.Services.Platforms.Audio
{
    public class WaveRecorderService : Service
    {
        private int _byteCount;
        private BinaryWriter _writer;
        private string _audioFilePath;
        private FileStream _fileStream;
        private IAudioStream _audioStream;
        private StreamWriter _streamWriter;

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
        /// Starts recording WAVE format audio from the audio stream.
        /// </summary>
        /// <param name="stream">A <see cref="IAudioStream"/> that provides the audio data.</param>
        /// <param name="filePath">The full path of the file to record audio to.</param>
        public void StartRecorder(IAudioStream stream, string filePath)
        {
            try
            {
                _byteCount = 0;
                _audioStream = stream;
                _audioFilePath = filePath;
                _fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                _streamWriter = new StreamWriter(_fileStream);
                _writer = new BinaryWriter(_streamWriter.BaseStream, Encoding.UTF8);
                _audioStream.Start(OnStreamBroadcast);
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
                StopRecorder();
                throw;
            }
        }

        /// <summary>
        /// Stops recording WAV audio from the underlying <see cref="IAudioStream"/> and finishes writing the WAV file.
        /// </summary>
        public void StopRecorder()
        {
            try
            {
                if (_writer != null)
                {
                    if (_streamWriter.BaseStream.CanWrite)
                    {
                        //now that audio is finished recording, write a WAV/RIFF header at the beginning of the file
                        _writer.Seek(0, SeekOrigin.Begin);
                        AudioFunctions.WriteWavHeader(
                            _writer,
                            _audioStream.ChannelCount,
                            _audioStream.SampleRate,
                            _audioStream.BitsPerSample,
                            _byteCount);
                    }

                    _writer.Dispose(); //this should properly close/dispose the underlying stream as well
                    _writer = null;
                    _fileStream = null;
                    _streamWriter = null;
                }

                _audioStream.Stop(OnStreamBroadcast);
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
                throw;
            }
        }

        private void OnStreamBroadcast(byte[] bytes)
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
                _loggerService.Log(ex);
                StopRecorder();
            }
        }
    }
}
