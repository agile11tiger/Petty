using Petty.Helpers;
namespace Petty.Services.Local;

public class NavigationService : Service
{
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

    public async Task GoBackAsync()
    {
        await GoToAsync($"..");
    }
}
