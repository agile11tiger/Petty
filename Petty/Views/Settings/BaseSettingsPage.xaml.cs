using Petty.Resources.Localization;
using Petty.ViewModels;
using Petty.ViewModels.Settings;

namespace Petty.Views.Settings;

public partial class BaseSettingsPage : ContentPage
{
	public BaseSettingsPage(AppShellViewModel appShellViewMode, BaseSettingsViewModel baseSettingsViewModel)
    {
        BindingContext = _baseSettingsViewModel = baseSettingsViewModel;
        _appShellViewModel = appShellViewMode;

        InitializeComponent();
        _pickerLanguage.SelectedIndex = baseSettingsViewModel.LanguagesDictionary
            .FindIndex(item => item.CultureInfo.Name == _baseSettingsViewModel.TempBaseSettings.LanguageType);
    }

    private readonly AppShellViewModel _appShellViewModel;
    private readonly BaseSettingsViewModel _baseSettingsViewModel;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _appShellViewModel.Title = AppResources.PageBaseSettings;
    }

    private void PickerLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
        _baseSettingsViewModel.TempBaseSettings.LanguageType = (_pickerLanguage.SelectedItem as Language).CultureInfo.Name;
    }
}