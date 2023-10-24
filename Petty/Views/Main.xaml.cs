namespace Petty.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel mainViewModel)
    {
        BindingContext = mainViewModel;
        Behaviors.Add(new EventToCommandBehavior { EventName = nameof(NavigatedTo), Command = mainViewModel.NavigatedToCommand });
        InitializeComponent();
    }
}