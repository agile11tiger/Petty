using Petty.Resources.Localization;
using Petty.ViewModels.Settings;

namespace Petty.Views.Settings;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(AppShellViewModel appShellViewModel, SettingsViewModel settingsViewModel)
    {
        BindingContext = settingsViewModel;
        _appShellViewModel = appShellViewModel;
        InitializeComponent();
    }

    private readonly AppShellViewModel _appShellViewModel;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _appShellViewModel.Title = AppResources.PageSettings;
    }
}