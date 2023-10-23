using Petty.Views.Controls.Magic.GradientFolder;
namespace Petty.Views.Controls.Magic.LinearGradientFolder;

public class LinearGradient : Gradient
{
    public LinearGradient()
    {
        _renderer = new LinearGradientRenderer(this);
    }

    private readonly LinearGradientRenderer _renderer;
    public double Angle { get => (double)GetValue(AngleProperty); set => SetValue(AngleProperty, value); }
    public static readonly BindableProperty AngleProperty = BindableProperty.Create(nameof(Angle), typeof(double), typeof(LinearGradient), 0.0);

    public override void Render(RenderContext context) => _renderer.Render(context);
    public override string ToString() => $"Angle={Angle}, Stops=LinearGradientStop[{Stops.Count}]";
}
