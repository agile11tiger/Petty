using System.Runtime.CompilerServices;

namespace Petty.Services.Local
{
    public class LoggerService
    {
        public void Log(
            string message = default,
            Exception exception = default,
            [CallerMemberName] string memberName = default,
            [CallerFilePath] string sourceFilePath = default,
            [CallerLineNumber] int sourceLineNumber = default)
        {
            return;
        }
    }
}
