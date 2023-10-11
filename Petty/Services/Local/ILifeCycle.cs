using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Local
{
    public interface ILifeCycle
    {
        bool IsStarting { get; }
        void Start();
        void Stop();
    }
}
