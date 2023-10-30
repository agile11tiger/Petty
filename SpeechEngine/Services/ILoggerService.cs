using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SpeechEngine.Services;

public interface ILoggerService
{
    void Log(
        Exception exception,
        LogLevel logLevel = LogLevel.Error,
        [CallerMemberName] string memberName = default,
        [CallerFilePath] string sourceFilePath = default,
        [CallerLineNumber] int sourceLineNumber = default);

    public void Log(
        string message,
        Exception exception = default,
        LogLevel logLevel = LogLevel.Error,
        [CallerMemberName] string memberName = default,
        [CallerFilePath] string sourceFilePath = default,
        [CallerLineNumber] int sourceLineNumber = default);
}
