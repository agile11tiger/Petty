namespace Petty.ViewModels.DisplayAlert;

public class RawLink(string text, int number = 0, bool isTitle = false) : ILink
{
    public string Text => text;
    public int Index => number;
    public bool IsTitle => isTitle;
}
