namespace Petty.Services.Local
{
    public class NavigationService
    {
        public NavigationService()
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
