namespace Petty.Services.Navigation
{
    public interface INavigationService
    {
        Task InitializeAsync();
        Task GoToAsync(string route, IDictionary<string, object> routeParameters = null);
        Task PopAsync();
        Task PopToMainAsync();
    }
}