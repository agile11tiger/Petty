namespace Petty.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel mainViewModel)
    {
        BindingContext = _mainViewModel = mainViewModel;
        InitializeComponent();
    }

    private readonly MainViewModel _mainViewModel;
}