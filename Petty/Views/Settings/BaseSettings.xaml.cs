using Petty.Resources.Localization;
using Petty.ViewModels.Settings;

namespace Petty.Views.Settings;

public partial class BaseSettingsPage : ContentPage
{
    public BaseSettingsPage(AppShellViewModel appShellViewMode, BaseSettingsViewModel baseSettingsViewModel)
    {
        BindingContext = baseSettingsViewModel;
        _appShellViewModel = appShellViewMode;
        InitializeComponent();
    }

    private readonly AppShellViewModel _appShellViewModel;

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _appShellViewModel.Title = AppResources.PageBaseSettings;
    }
}