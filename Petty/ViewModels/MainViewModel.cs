﻿using Petty.Helpres;
using Petty.Services.Local.Localization;
using Petty.ViewModels.Base;

namespace Petty.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public MainViewModel(
            LoggerService loggerService, 
            NavigationService navigationService,
            LocalizationService localizationService)
            : base(loggerService, navigationService, localizationService)
        {
        }

        [ObservableProperty]
        private bool _isSelectedTabBarItem;

        [RelayCommand]
        private async Task GoToSettingsAsync()
        {
            await NavigationService.GoToAsync(RoutesHelper.SETINGS);
            IsSelectedTabBarItem = false;
        }

        [RelayCommand]
        private async Task GoToLeaderboardAsync()
        {
            App.StartPettyGuadrAndroidService();
        }
    }
}
