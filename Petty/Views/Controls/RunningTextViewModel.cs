namespace Petty.Views.Controls;

public partial class RunningTextViewModel : ViewModelBase
{
    public RunningTextViewModel()
    {
        _runningText = string.Join(";", Enumerable.Range(1, 100));
        _runningTextStart = _runningText.Length * _runningTextStartCoefficient;
        _runningTextWidthRequest = _runningTextStart + 5; //+5 is length for last chars
        StartRunningTextThread();
    }

    private volatile bool _isStopRunningTextLine;
    private readonly double _runningTextStartCoefficient = 6.5;
    private readonly int _runningTextLeftBorderCoefficient = 55;
    private readonly AutoResetEvent _runningTextWaiter = new(false);
    [ObservableProperty] private string _runningText;
    [ObservableProperty] private double _runningTextStart;
    [ObservableProperty] private double _runningTextWidthRequest;

    private void StartRunningTextThread()
    {
        new Thread(() =>
        {
            try
            {
                _runningTextWaiter.WaitOne();
                var translationXInitialValue = RunningTextStart;

                while (true)
                {
                    RunningTextStart = translationXInitialValue;
                    var leftTextBorder = -translationXInitialValue / RunningText.Length * _runningTextLeftBorderCoefficient;

                    while (leftTextBorder < RunningTextStart--)
                    {
                        Thread.Sleep(15);

                        if (_isStopRunningTextLine)
                        {
                            RunningTextStart = translationXInitialValue;
                            _runningTextWaiter.WaitOne();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
            }
            finally
            {
                Monitor.Exit(_runningTextWaiter);
            }
        }).Start();
    }

    [RelayCommand]
    private void StartRunningText()
    {
        _isStopRunningTextLine = false;
        _runningTextWaiter.Set();
    }

    [RelayCommand]
    private void StopRunningText()
    {
        _isStopRunningTextLine = true;
    }
}
