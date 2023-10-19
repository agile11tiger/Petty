namespace Petty.Services.Local;

public interface ILifeCycle
{
    bool IsStarting { get; }
    void Start();
    void Stop();
}
