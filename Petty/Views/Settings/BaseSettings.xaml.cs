using Petty.ViewModels.Settings;
namespace Petty.Views.Settings;

public partial class BaseSettingsPage : ContentPage
{
    public BaseSettingsPage(BaseSettingsViewModel baseSettingsViewModel)
    {
        BindingContext = baseSettingsViewModel;
        Behaviors.Add(new EventToCommandBehavior { EventName = nameof(NavigatedTo), Command = baseSettingsViewModel.NavigatedToCommand });
        InitializeComponent();
    }
}