using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Petty.Services.Local
{
    public class LoggerService
    {
        public void Log(
            Exception exception = default,
            [CallerMemberName] string memberName = default,
            [CallerFilePath] string sourceFilePath = default,
            [CallerLineNumber] int sourceLineNumber = default)
        {
            Log(null, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        public void Log(
            string message = default,
            Exception exception = default,
            [CallerMemberName] string memberName = default,
            [CallerFilePath] string sourceFilePath = default,
            [CallerLineNumber] int sourceLineNumber = default)
        {
            Debug.WriteLine($"{DateTime.UtcNow}. {sourceFilePath}. {memberName}. {sourceLineNumber}.\r\n{message}\r\n{exception}");
        }

    }
}
