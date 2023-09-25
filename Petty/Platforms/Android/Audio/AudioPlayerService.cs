using Android.Media;

namespace Petty.Services.Platforms.Audio
{
    /// <summary>
    /// https://xamarin.ru/knowledge-base/xamarin-android/mediaplayer/
    /// </summary>
    public partial class AudioPlayerService
    {
        public AudioPlayerService()
        {
        }

        private MediaPlayer _mediaPlayer;

        public void Play(string pathToAudioFile)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Completion -= MediaPlayer_Completion;
                _mediaPlayer.Stop();
            }

            if (pathToAudioFile != null)
            {
                if (_mediaPlayer == null)
                {
                    _mediaPlayer = new MediaPlayer();
                    _mediaPlayer.Prepared += (sender, args) =>
                    {
                        _mediaPlayer.Start();
                        _mediaPlayer.Completion += MediaPlayer_Completion;
                    };
                }

                _mediaPlayer.Reset();
                //_mediaPlayer.SetVolume (1.0f, 1.0f);

                _mediaPlayer.SetDataSource(pathToAudioFile);
                _mediaPlayer.PrepareAsync();
            }
        }

        public void Pause()
        {
            _mediaPlayer?.Pause();
        }

        private void MediaPlayer_Completion(object sender, EventArgs e)
        {
            FinishedPlaying?.Invoke(this, EventArgs.Empty);
        }
    }
}
