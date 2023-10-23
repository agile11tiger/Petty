using Petty.Views.Controls.Magic.CssParser;
using Petty.Views.Controls.Magic.GradientFolder;

namespace Petty.Views.Controls.Magic;

[ContentProperty("Stylesheet")]
public class CssGradientSource : BindableObject, IGradientSource
{
    public static readonly BindableProperty StylesheetProperty = BindableProperty.Create(nameof(Stylesheet), typeof(string), typeof(CssGradientSource));
    public string Stylesheet { get => (string)GetValue(StylesheetProperty); set => SetValue(StylesheetProperty, value); }
    public IEnumerable<Gradient> GetGradients() => new CssGradientParser().ParseCss(Stylesheet);
}
