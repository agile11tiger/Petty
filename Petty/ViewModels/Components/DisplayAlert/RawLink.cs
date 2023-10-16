namespace Petty.ViewModels.Components.DisplayAlert
{
    public class RawLink(string text, bool isTitle = false) : ILink
    {
        public string Text => text;
        public bool IsTitle => isTitle;
        public bool IsRawLink => true;
    }
}
