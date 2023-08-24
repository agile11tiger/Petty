using Petty.Services.Navigation;
using Petty.Views;

namespace Petty
{
    public partial class AppShell : Shell
    {
        public AppShell(INavigationService navigationService)
        {
            _navigationService = navigationService;
            InitializeComponent();
        }

        private readonly INavigationService _navigationService;

        protected override async void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler is not null)
            {
                await _navigationService.InitializeAsync();
            }
        }
    }
}
