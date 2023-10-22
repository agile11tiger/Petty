namespace Petty.Views;

public partial class SplashScreenPage : ContentPage
{
    public SplashScreenPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _yinYangSpinner.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _yinYangSpinner.OnDisappearing();
    }
}
