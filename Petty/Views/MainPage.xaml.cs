using Petty.Resources.Localization;
using Petty.Services.Local.PermissionsFolder;
using Petty.ViewModels;

namespace Petty.Views;

public partial class MainPage : ContentPage
{
    public MainPage(
        MainViewModel mainViewModel,
        AppShellViewModel appShellViewModel, 
        PermissionService permissionService)
    {
        BindingContext = mainViewModel;
        _appShellViewModel = appShellViewModel;
        _permissionService = permissionService;
        InitializeComponent();
    }

    private readonly PermissionService _permissionService;
    private readonly AppShellViewModel _appShellViewModel;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _appShellViewModel.Title = AppResources.PagePetty;
        await _permissionService.GetAllPermissionsAsync();
    }
}