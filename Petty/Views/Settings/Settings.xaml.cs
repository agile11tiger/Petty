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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _appShellViewModel.Title = AppResources.PageSettings;
    }
}