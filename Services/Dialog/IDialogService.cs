using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Dialog
{
    internal interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel);
    }
}
