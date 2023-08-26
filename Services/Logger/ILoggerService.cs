using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Logger
{
    public interface ILoggerService
    {
        void Log(
            string message = default,
            Exception ex = default,
            [CallerMemberName] string memberName = default,
            [CallerFilePath] string sourceFilePath = default,
            [CallerLineNumber] int sourceLineNumber = default);
    }
}
