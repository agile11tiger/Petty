using Android.Content;
using Android.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Local.Audio
{
    public partial class AudioRecorderService
    {
        public void Initialize()
        {
            if (Android.OS.Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.JellyBean)
            {
                try
                {
                    //if the below call to AudioManager is blocking and never returning/taking forever, ensure the emulator has proper access to the system mic input
                    var audioManager = (AudioManager)global::Android.App.Application.Context.GetSystemService(Context.AudioService);
                    var property = audioManager.GetProperty(AudioManager.PropertyOutputSampleRate);

                    if (!string.IsNullOrEmpty(property) && int.TryParse(property, out int sampleRate))
                        PreferredSampleRate = sampleRate;
                }
                catch (Exception ex)
                {
                    _logger.Log("Error using AudioManager to get AudioManager.PropertyOutputSampleRate. " +
                        "PreferredSampleRate will remain at the default", ex);
                }
            }
        }

        private Task<string> GetDefaultFilePath()
        {
            return Task.FromResult(Path.Combine(Path.GetTempPath(), DEFAULT_FILE_NAME));
        }

        private void OnRecordingStarting()
        {
        }

        private void OnRecordingStopped()
        {
        }
    }
}
