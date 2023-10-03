using Petty.Resources.Localization;

namespace Petty.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel settingsViewModel, AppShellViewModel appShellViewModel)
    {
        BindingContext = _settingsViewModel = settingsViewModel;
        _appShellViewModel = appShellViewModel;
        InitializeComponent();
        _pickerLanguage.SelectedIndex = _settingsViewModel.LanguagesDictionary
            .FindIndex(item => item.CultureInfo.Name == _settingsViewModel.TempSettings.LanguageType);
    }

    private readonly SettingsViewModel _settingsViewModel;
    private readonly AppShellViewModel _appShellViewModel;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _appShellViewModel.Title = AppResources.PageSettings;
    }

    private void PickerLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
        _settingsViewModel.TempSettings.LanguageType = (_pickerLanguage.SelectedItem as Language).CultureInfo.Name;
    }
}