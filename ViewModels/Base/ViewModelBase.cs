using Microsoft.Extensions.Logging;
using Petty.Services.Logger;
using Petty.Services.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.ViewModels.Base
{
    public abstract partial class ViewModelBase : ObservableObject, IViewModelBase
    {
        private long _isBusy;
        [ObservableProperty]
        private bool _isInitialized;

        public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;

        public ILoggerService LoggerService { get; }
        public INavigationService NavigationService { get; }

        public IAsyncRelayCommand InitializeAsyncCommand { get; }

        public ViewModelBase(
            ILoggerService logger,
            INavigationService navigationService)
        {
            LoggerService = logger;
            NavigationService = navigationService;
            InitializeAsyncCommand =
                new AsyncRelayCommand(
                    async () =>
                    {
                        await IsBusyFor(InitializeAsync);
                        IsInitialized = true;
                    },
                    AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task IsBusyFor(Func<Task> executeTask)
        {
            Interlocked.Increment(ref _isBusy);
            OnPropertyChanged(nameof(IsBusy));

            try
            {
                await executeTask();
            }
            finally
            {
                Interlocked.Decrement(ref _isBusy);
                OnPropertyChanged(nameof(IsBusy));
            }
        }
    }
}
