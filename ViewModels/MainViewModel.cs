using Petty.Services.Navigation;
using Petty.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
