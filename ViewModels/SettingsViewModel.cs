using Petty.Models.Settings;
using Petty.ViewModels.Base;

namespace Petty.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(
            LoggerService loggerService,
            NavigationService navigationService,
            LocalizationService localizationService)
            : base(loggerService, navigationService, localizationService)
        {
        }


        [ObservableProperty] private Settings _settings;
        private Settings _tempSettings;
        private string _slider3Text;
        private double _slider3Value;

        #region Properties(OnPropertyChanged)

        public string Slider3Text
        {
            get => LocalizationService.Get(nameof(Slider3Text), Slider3Value);
            set => _slider3Text = value;
        }

        public double Slider3Value
        {
            get => _slider3Value;
            set
            {
                if (SetProperty(ref _slider3Value, value))
                    OnPropertyChanged(nameof(Slider3Text));
            }
        }
        public bool IsSoundShutterRelease
        {
            get => true;
            set => value = true;
        }

        public bool? UseFrontCamera
        {
            get => false;
            set => value = false;
        }

        public bool? TryHarder
        {
            get => true;
            set => value = true;
        }

        public bool? TryInverted
        {
            get => false;
            set => value = false;
        }

        #endregion

        [RelayCommand]
        private Task OnDisappearing()
        {
            _tempSettings = Settings.Clone();
            return Task.FromResult(true);
        }

        [RelayCommand]
        private Task MakeDefaultSettings()
        {
            Settings = new Settings();
            _tempSettings = new Settings();
            return GoToMainPageAsync();
        }

        [RelayCommand]
        private async Task ApplySettings()
        {
            Settings = _tempSettings.Clone();
            await GoToMainPageAsync().ConfigureAwait(false);
        }

        private async Task GoToMainPageAsync()
        {
            await NavigationService.PopToMainAsync().ConfigureAwait(false);
            //await AsyncDatabase.AddOrReplaceItemAsync(Settings);
        }
    }
}
