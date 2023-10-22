using static System.Net.Mime.MediaTypeNames;
namespace Petty.ViewModels.DisplayAlert;

public class PickerLink(string text, int number = 0, string selectedLinkText = null, bool isTitle = false) 
    : RawLink(text, number, isTitle)
{
    public string SelectedLinkText => selectedLinkText ?? Text;
}
