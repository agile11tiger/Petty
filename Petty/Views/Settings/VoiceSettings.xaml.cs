using Petty.ViewModels.Settings;
namespace Petty.Views.Settings;

public partial class VoiceSettingsPage : ContentPage
{
    public VoiceSettingsPage(VoiceSettingsViewModel voiceSettings)
    {
        BindingContext = voiceSettings;
        Behaviors.Add(new EventToCommandBehavior { EventName = nameof(NavigatedTo), Command = voiceSettings.NavigatedToCommand });
        InitializeComponent();
    }
}