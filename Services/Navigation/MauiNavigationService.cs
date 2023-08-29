using Petty.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Navigation
{
    internal class MauiNavigationService: INavigationService
    {
        public MauiNavigationService()
        {
        }

        //TODO: change
        public Task InitializeAsync() => GoToAsync(
            string.IsNullOrEmpty("_settingsService.AuthAccessToken") ? "//Login" : "//Main");

        public Task GoToAsync(string route, IDictionary<string, object> routeParameters = null)
        {
            var shellNavigation = new ShellNavigationState(route);

            return routeParameters != null
                ? Shell.Current.GoToAsync(shellNavigation, routeParameters)
                : Shell.Current.GoToAsync(shellNavigation);
        }

        public Task PopAsync() => Shell.Current.GoToAsync("..");

        public async Task PopToMainAsync()
        {
            await GoToAsync("//Main");
        }
    }
}
