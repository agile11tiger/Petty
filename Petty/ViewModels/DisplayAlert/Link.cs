namespace Petty.ViewModels.DisplayAlert;

public class Link(List<string> textParts, int number = 0, Func<Task> action = null) : ILink
{
    public int Index => number;
    public string Name => TextParts[0];
    public Func<Task> Action => action;
    public string Description => TextParts[1];
    public List<string> TextParts => textParts;
}
