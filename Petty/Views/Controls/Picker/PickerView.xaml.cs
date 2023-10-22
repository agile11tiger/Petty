using Microsoft.Maui.Controls;
using Mopups.Pages;
using Petty.Resources.Localization;
using Petty.Services.Local.UserMessages;
using Petty.ViewModels.DisplayAlert;
using Petty.Views.Controls.CircularProgressBar;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
namespace Petty.Views.Controls.Picker;

public partial class PickerView : ContentView
{
	public PickerView()
	{
		InitializeComponent();
        _closingCommand = new Command<ILink>(HandleClosing);
        _userMessagesService = MauiProgram.ServiceProvider.GetService<UserMessagesService>();
    }

    private DisplayAlertPage _pickerPage;
    private readonly Command<ILink> _closingCommand;
    private readonly UserMessagesService _userMessagesService;
    private static readonly Color _selectedItemNameUnderlineColorDefault = Color.FromRgb(238, 238, 238);
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(PickerView));
    public static readonly BindableProperty SelectedItemNameProperty = BindableProperty.Create(nameof(SelectedItemName), typeof(string), typeof(PickerView));
    public static readonly BindableProperty ClosingCommandProperty = BindableProperty.Create(nameof(ClosingCommand), typeof(ICommand), typeof(PickerView));
    public static readonly BindableProperty SelectedItemNameUnderlineColorProperty = BindableProperty.Create(nameof(SelectedItemNameUnderlineColor), typeof(Color), typeof(PickerView), _selectedItemNameUnderlineColorDefault);

    public IList ItemsSource
    {
        get => (IList)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string SelectedItemName
    {
        get => (string)GetValue(SelectedItemNameProperty);
        set => SetValue(SelectedItemNameProperty, value);
    }

    public ICommand ClosingCommand
    {
        get => (ICommand)GetValue(ClosingCommandProperty);
        set => SetValue(ClosingCommandProperty, value);
    }

    public Color SelectedItemNameUnderlineColor
    {
        get => (Color)GetValue(SelectedItemNameUnderlineColorProperty);
        set => SetValue(SelectedItemNameUnderlineColorProperty, value);
    }

    private async void ShowPickerAsync(object sender, TappedEventArgs e)
    {
        if (App.Current.Resources.TryGetValue("Primary", out object color))
            SelectedItemNameUnderlineColor = (Color)color;

        if (_pickerPage == null)
        {
            _pickerPage = await _userMessagesService.CreateDisplayAlertPageAsync(
                ItemsSource, cancel:
                AppResources.ButtonCancel,
                selectionMode: SelectionMode.Single);
            _pickerPage.DisplayAlertViewModel.ClosingCommand = _closingCommand;
        }

        await _userMessagesService.SendMessageAsync(_pickerPage, true);
    }

    private void HandleClosing(ILink link)
    {
        ClosingCommand?.Execute(link);
        SelectedItemNameUnderlineColor = _selectedItemNameUnderlineColorDefault;
    }
}