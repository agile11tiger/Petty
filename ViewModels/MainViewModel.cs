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
    public partial class MainViewModel : ViewModelBase
    {
        public MainViewModel(ILoggerService loggerService, INavigationService navigationService) 
            : base(loggerService, navigationService)
        {
        }

        [ObservableProperty]
        private bool _isSelectedTabBarItem;

        [RelayCommand]
        private async Task GoToSettingsAsync()
        {
            await NavigationService.GoToAsync("Settings");
            IsSelectedTabBarItem = false;
        }
    }
}
