namespace Petty.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _settingsViewModel;

    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        BindingContext = _settingsViewModel = settingsViewModel;
        InitializeComponent();
    }

    private void SomeNumbers_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        someNumbers1.Text = string.Format("Выбрано: {0:F1}", e.NewValue);
        someNumbers2.Text = string.Format("Выбрано: {0:F1}", e.NewValue);
        someNumbers3.Text = string.Format("Выбрано: {0:F1}", e.NewValue);
    }
}