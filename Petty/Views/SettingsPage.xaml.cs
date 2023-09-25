namespace Petty.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        BindingContext = _settingsViewModel = settingsViewModel;
        InitializeComponent();
        _pickerLanguage.SelectedIndex = _settingsViewModel.LanguagesDictionary
            .FindIndex(item => item.CultureInfo.Name == _settingsViewModel.TempSettings.LanguageType);
    }

    private readonly SettingsViewModel _settingsViewModel;

    private void pickerLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
        _settingsViewModel.TempSettings.LanguageType = (_pickerLanguage.SelectedItem as Language).CultureInfo.Name;
    }
}