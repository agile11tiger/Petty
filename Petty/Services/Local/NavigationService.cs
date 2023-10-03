using Petty.Helpres;

namespace Petty.Services.Local
{
    public class NavigationService : Service
    {
        public NavigationService(LoggerService loggerService)
            : base(loggerService)
        {
        }

        //TODO: change
        public Task InitializeAsync() => GoToAsync(
            string.IsNullOrEmpty("_settingsService.AuthAccessToken") ? "Login" : "Main");

        public Task GoToAsync(string route, IDictionary<string, object> routeParameters = null)
        {
            var shellNavigation = new ShellNavigationState(route);

            return routeParameters != null
                ? Shell.Current.GoToAsync(shellNavigation, true, routeParameters)
                : Shell.Current.GoToAsync(shellNavigation, true);
        }

        public async Task PopToMainAsync()
        {
            await GoToAsync($"//{RoutesHelper.MAIN}");
        }
    }
}
