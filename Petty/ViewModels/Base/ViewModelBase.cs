namespace Petty.ViewModels.Base;

public abstract partial class ViewModelBase : ObservableObject
{
    public ViewModelBase()
    {
        _loggerService = MauiProgram.ServiceProvider.GetService<LoggerService>();
        _navigationService = MauiProgram.ServiceProvider.GetService<NavigationService>();
        _localizationService = MauiProgram.ServiceProvider.GetService<LocalizationService>();
        _settingsService = MauiProgram.ServiceProvider.GetService<SettingsService>();
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
    private readonly AppShellViewModel _appShellViewModelCache;
    protected readonly LoggerService _loggerService;
    protected readonly SettingsService _settingsService;
    protected readonly NavigationService _navigationService;
    protected readonly LocalizationService _localizationService;
    protected readonly IAsyncRelayCommand _initializeAsyncCommand;
    protected PettySQLite.Models.Settings _settings => _settingsService.Settings;
    protected AppShellViewModel _appShellViewModel => _appShellViewModelCache ?? MauiProgram.ServiceProvider.GetService<AppShellViewModel>();
    [ObservableProperty] private bool _isInitialized;
    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;

    public void HapticFeedback()
    {
        if (_settings.BaseSettings.IsHapticFeedback)
            Microsoft.Maui.Devices.HapticFeedback.Default.Perform(HapticFeedbackType.Click);
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
