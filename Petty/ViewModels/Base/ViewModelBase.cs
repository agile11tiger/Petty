namespace Petty.ViewModels.Base
{
    public abstract partial class ViewModelBase : ObservableObject
    {
        public ViewModelBase()
        {
            _loggerService = MauiProgram.ServiceProvider.GetService<LoggerService>();
            _navigationService = MauiProgram.ServiceProvider.GetService<NavigationService>();
            _localizationService = MauiProgram.ServiceProvider.GetService<LocalizationService>();
            _initializeAsyncCommand =
                new AsyncRelayCommand(
                    async () =>
                    {
                        await IsBusyFor(InitializeAsync);
                        IsInitialized = true;
                    },
                    AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
        }

        private long _isBusy;
        protected LoggerService _loggerService;
        protected NavigationService _navigationService;
        protected LocalizationService _localizationService;
        protected IAsyncRelayCommand _initializeAsyncCommand;
        [ObservableProperty] private bool _isInitialized;
        public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;

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
