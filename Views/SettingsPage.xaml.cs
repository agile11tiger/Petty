using Petty.Resources.Localization;

namespace Petty.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _settingsViewModel;

    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        BindingContext = _settingsViewModel = settingsViewModel;
        InitializeComponent();
    }

}