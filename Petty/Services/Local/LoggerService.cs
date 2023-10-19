using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace Petty.Services.Local;

public class LoggerService
{
    public void Log(
        Exception exception,
        LogLevel logLevel = LogLevel.Error,
        [CallerMemberName] string memberName = default,
        [CallerFilePath] string sourceFilePath = default,
        [CallerLineNumber] int sourceLineNumber = default)
    {
        Log(null, exception, logLevel, memberName, sourceFilePath, sourceLineNumber);
    }

    public void Log(
        string message,
        Exception exception = default,
        LogLevel logLevel = LogLevel.Error,
        [CallerMemberName] string memberName = default,
        [CallerFilePath] string sourceFilePath = default,
        [CallerLineNumber] int sourceLineNumber = default)
    {
        Debug.WriteLine($"{DateTime.UtcNow}. {sourceFilePath}. {memberName}. {sourceLineNumber}.\r\n{message}\r\n{exception}");
    }

}
