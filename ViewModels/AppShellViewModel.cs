using Petty.Services.Logger;
using Petty.Services.Navigation;
using Petty.ViewModels.Base;
using Petty.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Petty.ViewModels
{
    public partial class AppShellViewModel : ViewModelBase
    {
        public AppShellViewModel(
            ILoggerService loggerService,
            INavigationService navigationService,
            RunningTextViewModel runningTextViewModel) 
            : base(loggerService, navigationService)
        {
            _runningTextViewModel = runningTextViewModel;
        }

        [ObservableProperty] private RunningTextViewModel _runningTextViewModel;
        [ObservableProperty] private bool _isAnimationPlayingCoffeeGif;

        [RelayCommand]
        private async Task StartCoffeGifAsync()
        {
            await Task.Run(async() =>
            {
                await Task.Delay(3000);
                IsAnimationPlayingCoffeeGif = true;
            });
        }

        [RelayCommand]
        private void StopCoffeGif()
        {
            IsAnimationPlayingCoffeeGif = false;
        }
    }
}
