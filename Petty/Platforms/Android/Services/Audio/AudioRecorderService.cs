namespace Petty.Services.Platforms.Audio
{
    public partial class AudioRecorderService
    {
        private static volatile int _sampleRateCache;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// it`s working, but no need now, see <see cref="PreferredSampleRate"/>
        /// </remarks>
        public void Initialize()
        {
            //  
            //if (Android.OS.Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.JellyBean)
            //{
            //    lock (_locker)
            //    {
            //        if (_sampleRateCache != default)
            //        {
            //            PreferredSampleRate = _sampleRateCache;
            //            return;
            //        }

            //        try
            //        {
            //            //if the below call to AudioManager is blocking and never returning/taking forever, ensure the emulator has proper access to the system mic input
            //            var audioManager = (AudioManager)global::Android.App.Application.Context.GetSystemService(Context.AudioService);
            //            var property = audioManager.GetProperty(AudioManager.PropertyOutputSampleRate);

            //            if (!string.IsNullOrEmpty(property) && int.TryParse(property, out int sampleRate))
            //                PreferredSampleRate = sampleRate;
            //        }
            //        catch (Exception ex)
            //        {
            //            _loggerService.Log("Error using AudioManager to get AudioManager.PropertyOutputSampleRate. " +
            //                "PreferredSampleRate will remain at the default", ex);
            //        }
            //    }
            //}
        }
    }
}
