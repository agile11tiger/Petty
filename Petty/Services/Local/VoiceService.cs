using Petty.Services.Platforms.PettyCommands.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Local
{
    public class VoiceService : Service
    {
        public VoiceService(LoggerService loggerService, SettingsService settingsService) : base(loggerService)
        {
            _tokenSource = new CancellationTokenSource();
            _speechOptions = new SpeechOptions()
            {
                Pitch = (float)settingsService.Settings.VoiceSettings.Pitch,// 0.0 - 2.0
                Volume = (float)settingsService.Settings.VoiceSettings.Volume, // 0.0 - 1.0
            };
        }

        private SpeechOptions _speechOptions;
        private CancellationTokenSource _tokenSource;

        public async Task SpeakAsync(string speech)
        {
            await TextToSpeech.Default.SpeakAsync(speech, _speechOptions, _tokenSource.Token);
        }

        public async Task SpeakAsync(string speech, VoiceSettings voiceSettings)
        {
            var speechOptions = new SpeechOptions
            {
                Pitch = voiceSettings.Pitch,
                Volume = voiceSettings.Volume
            };
            await TextToSpeech.Default.SpeakAsync(speech, speechOptions, _tokenSource.Token);
        }

        public void CancelSpeech()
        {
            if (_tokenSource.IsCancellationRequested)
                return;

            _tokenSource.Cancel();
        }

        public void PlayCommandExecutionFailed(IPettyCommand command)
        {

        }

        public void PlayCommandExecutionSuccessed(IPettyCommand command)
        {

        }
    }
}
