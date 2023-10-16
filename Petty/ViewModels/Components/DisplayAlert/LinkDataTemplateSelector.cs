namespace Petty.ViewModels.Components.DisplayAlert
{
    public class LinkDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LinkTemplate { get; set; }
        public DataTemplate LinkTextTemplate { get; set; }
        public DataTemplate RawLinkTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item is RawLink
                ? RawLinkTemplate
                : ((Link)item).Action != null ? LinkTemplate : LinkTextTemplate;
        }
    }
}
