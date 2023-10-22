namespace Petty;

public partial class AppShell : Shell
{
    public AppShell(AppShellViewModel appShellViewModel)
    {
        BindingContext = appShellViewModel;
        InitializeComponent();
        appShellViewModel.InvalidateProgressBar = _circularProgressBarGraphicsView.Invalidate;
    }
}
