using Petty.Resources.Localization;

namespace Petty.Views;

public partial class SpeechSimulatorPage : ContentPage
{
	public SpeechSimulatorPage(SpeechSimulatorViewModel speechSimulatorViewModel, AppShellViewModel appShellViewModel)
	{
        BindingContext = _speechSimulatorViewModel = speechSimulatorViewModel;
        _appShellViewModel = appShellViewModel;
        InitializeComponent();
    }

    private readonly AppShellViewModel _appShellViewModel;
    private readonly SpeechSimulatorViewModel _speechSimulatorViewModel;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(10); //Otherwise, the question mark is shown before moving to a new page.
        _appShellViewModel.Title = AppResources.SpeechSimulator;
        _appShellViewModel.IsVisibleQuestionIcon = true;
        _appShellViewModel.ShowQuestionIconInfo = _speechSimulatorViewModel.ShowQuestionIconInfoCommand;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _appShellViewModel.IsVisibleQuestionIcon = false;
        _appShellViewModel.ShowQuestionIconInfo = null;
    }
}