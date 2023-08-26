using Petty.Services.Logger;
using Petty.Services.Navigation;
using Petty.ViewModels.Base;
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
            INavigationService navigationService) 
            : base(navigationService)
        {
            _loggerService = loggerService;
            _navigationService = navigationService;
            _runningText = string.Join(";", Enumerable.Range(1, 100));
            _runningTextStart = _runningText.Length * _runningTextStartCoefficient;
            _runningTextMaximumWidthRequest = _runningTextStart + 5; //+ length for last chars
        }

        [ObservableProperty] private string _runningText;
        [ObservableProperty] private double _runningTextStart;
        [ObservableProperty] private double _runningTextMaximumWidthRequest;
        private volatile bool _isStopRunningTextLine;
        private readonly ILoggerService _loggerService;
        private readonly INavigationService _navigationService;
        private readonly double _runningTextStartCoefficient = 6.5;
        private readonly int _runningTextLeftBorderCoefficient = 55;

        [RelayCommand]
        private void StartRunningText()
        {
            var thread = new Thread(() =>
            {
                try
                {
                    var translationXInitialValue = RunningTextStart;

                    while (true)
                    {
                        RunningTextStart = translationXInitialValue;
                        var leftTextBorder = -translationXInitialValue / RunningText.Length * _runningTextLeftBorderCoefficient;

                        while (leftTextBorder < RunningTextStart--)
                        {
                            Thread.Sleep(15);

                            if (_isStopRunningTextLine)
                                return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _loggerService.Log(default, ex);
                }
            });
            thread.Start();
        }

        [RelayCommand]
        private void StopRunningText()
        {
            _isStopRunningTextLine = true;
        }
    }
}
