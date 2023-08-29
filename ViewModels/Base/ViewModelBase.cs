namespace Petty.ViewModels.Base
{
    public abstract partial class ViewModelBase : ObservableObject, IViewModelBase
    {
        private long _isBusy;
        [ObservableProperty]
        private bool _isInitialized;

        public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;

        public LoggerService LoggerService { get; }
        public NavigationService NavigationService { get; }
        public LocalizationService LocalizationService { get; }

        public IAsyncRelayCommand InitializeAsyncCommand { get; }

        public ViewModelBase(
            LoggerService logger,
            NavigationService navigationService,
            LocalizationService localizationService)
        {
            LoggerService = logger;
            NavigationService = navigationService;
            LocalizationService = localizationService;
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
