using Petty.Models.Settings;
using Petty.Services.Logger;
using Petty.Services.Navigation;
using Petty.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(ILoggerService loggerService, INavigationService navigationService) 
            : base(loggerService, navigationService)
        {
        }


        [ObservableProperty] private Settings _settings;
        [ObservableProperty] private double _someNumbers;
        private Settings _tempSettings;

        #region Properties(OnPropertyChanged)

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
