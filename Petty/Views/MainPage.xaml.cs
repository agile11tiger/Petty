using Petty.Services.Local.PermissionsFolder;

namespace Petty.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel mainViewModel, PermissionService permissionService)
    {
        BindingContext = mainViewModel;
        _permissionService = permissionService;
        InitializeComponent();
    }

    private readonly PermissionService _permissionService;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _permissionService.GetAllPermissionsAsync();
    }
}