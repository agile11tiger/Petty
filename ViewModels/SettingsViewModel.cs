using Petty.Services.Navigation;
using Petty.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
