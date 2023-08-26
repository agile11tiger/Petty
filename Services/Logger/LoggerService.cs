using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Logger
{
    public class LoggerService : ILoggerService
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
