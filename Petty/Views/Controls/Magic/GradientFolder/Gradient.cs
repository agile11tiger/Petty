using Petty.Views.Controls.Magic.LinearGradientFolder;
namespace Petty.Views.Controls.Magic.GradientFolder;

[ContentProperty("Stops")]
public abstract class Gradient : BindableObject, IGradientSource
{
    public static readonly BindableProperty IsRepeatingProperty = BindableProperty.Create(nameof(IsRepeating), typeof(bool), typeof(LinearGradient), false);
    public bool IsRepeating { get => (bool)GetValue(IsRepeatingProperty); set => SetValue(IsRepeatingProperty, value); }
    public IList<GradientStop> Stops { get; set; } = new List<GradientStop>();
    public IEnumerable<Gradient> GetGradients() => new List<Gradient> { this };
    public abstract void Render(RenderContext context);
}