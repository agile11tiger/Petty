namespace Petty.ViewModels.Components.DisplayAlert
{
    public class Link(List<string> textParts, Func<Task> action = null) : ILink
    {
        public List<string> TextParts => textParts;
        public Func<Task> Action => action;
        public string Number => TextParts[0];
        public string Name => TextParts[1];
        public string Description => TextParts[2];
    }
}
