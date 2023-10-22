using Petty.Resources.Localization;
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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _appShellViewModel.Title = AppResources.PageVoiceSettings;
    }
}