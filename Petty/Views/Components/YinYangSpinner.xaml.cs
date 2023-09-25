namespace Petty.Views.Components;

public partial class YinYangSpinner : ContentView
{
    public YinYangSpinner()
    {
        InitializeComponent();
        _loggerService = MauiProgram.ServiceProvider.GetService<LoggerService>();
    }

    private long _isLooping;
    private readonly LoggerService _loggerService;

    /// <summary>
    /// TODO: override when its appear
    /// https://github.com/dotnet/maui/issues/805
    /// https://stackoverflow.com/questions/60448708/is-there-a-way-to-implement-an-on-loaded-event-for-a-content-view-in-xamarin
    /// </summary>
    public async void OnAppearing()
    {
        await Task.Run(async () =>
        {
            try
            {
                while (Interlocked.Read(ref _isLooping) < 1)
                {
                    if (_spinner.CanvasSize.IsEmpty)
                    {
                        //A delay is needed because appearing occurs before rendering.
                        await Task.Delay(1000);
                        continue;
                    }

                    _spinner.InvalidateSurface();
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
            }
        });
    }

    public void OnDisappearing()
    {
        Interlocked.Increment(ref _isLooping);
    }
}