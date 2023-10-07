using Petty.Resources.Localization;
using Petty.ViewModels;
using Petty.ViewModels.Settings;

namespace Petty.Views.Settings;

public partial class VoiceSettingsPage : ContentPage
{
    public VoiceSettingsPage(AppShellViewModel appShellViewModel, VoiceSettingsViewModel voiceSettings)
	{
        BindingContext = voiceSettings;
        InitializeComponent();
        _appShellViewModel = appShellViewModel;
    }

    private readonly AppShellViewModel _appShellViewModel;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _appShellViewModel.Title = AppResources.PageVoiceSettings;
    }
}