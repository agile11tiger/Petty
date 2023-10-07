using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Petty.Extensions;
using Petty.Resources.Localization;
using Petty.Services.Local;
using Petty.Services.Local.Localization;
using Petty.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.ViewModels.Settings
{
    public partial class VoiceSettingsViewModel : ViewModelBase
    {
        public VoiceSettingsViewModel(
            VoiceService voiceService,
            LoggerService loggerService,
            DatabaseService databaseService,
            SettingsService settingsService,
            NavigationService navigationService,
            LocalizationService localizationService) 
            : base(loggerService, navigationService, localizationService)
        {
            _voiceService = voiceService;
            _databaseService = databaseService;
            _settingsService = settingsService;
            _tempVoiceSettings = settingsService.Settings.VoiceSettings.CloneJson();
        }

        private VoiceSettings _tempVoiceSettings;
        private readonly VoiceService _voiceService;
        private readonly DatabaseService _databaseService;
        private readonly SettingsService _settingsService;
        [ObservableProperty] private string _speech = AppResources.EditorVoiceTestSpeech;

        //https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device-media/text-to-speech?tabs=android#:~:text=Maximum-,Pitch,-0
        public float PitchMaxValue { get => 2; }
        public float VolumeMaxValue { get => 1; }

        public string PitchValueText
        {
            get => _localizationService.Get(nameof(AppResources.LabelSettingsValue), PitchValue);
        }

        public float PitchValue
        {
            get => (float)Math.Round(_tempVoiceSettings.Pitch, 2);
            set
            {
                if (EqualityComparer<double>.Default.Equals(_tempVoiceSettings.Pitch, value))
                    return;

                _tempVoiceSettings.Pitch = value;
                OnPropertyChanged(nameof(PitchValue));
                OnPropertyChanged(nameof(PitchValueText));
            }
        }

        public string VolumeValueText
        {
            get => _localizationService.Get(nameof(AppResources.LabelSettingsValue), VolumeValue);
        }

        public float VolumeValue
        {
            get => (float)Math.Round(_tempVoiceSettings.Volume, 2);
            set
            {
                if (EqualityComparer<double>.Default.Equals(_tempVoiceSettings.Volume, value))
                    return;

                _tempVoiceSettings.Volume = value;
                OnPropertyChanged(nameof(VolumeValue));
                OnPropertyChanged(nameof(VolumeValueText));
            }
        }

        [RelayCommand]
        private async Task SpeakAsync()
        {
            await _voiceService.SpeakAsync(Speech, _tempVoiceSettings);
        }

        [RelayCommand]
        private Task ApplyDefaultSettings()
        {
            var _voiceSttingsDefault = new PettySQLite.Models.VoiceSettings();
            VolumeValue = _voiceSttingsDefault.Volume;
            PitchValue = _voiceSttingsDefault.Pitch;
            return GoBackAsync();
        }

        [RelayCommand]
        private async Task ApplySettings()
        {
            await GoBackAsync();
        }

        private async Task GoBackAsync()
        {
            await _databaseService.CreateOrUpdateAsync(_tempVoiceSettings);
            _settingsService.Settings.VoiceSettings = _tempVoiceSettings;
            await _navigationService.GoBackAsync();
        }
    }
}
