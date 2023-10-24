using Petty.ViewModels.Settings;
namespace Petty.Views.Settings;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        BindingContext = settingsViewModel;
        Behaviors.Add(new EventToCommandBehavior { EventName = nameof(NavigatedTo), Command = settingsViewModel.NavigatedToCommand });
        InitializeComponent();
    }
}