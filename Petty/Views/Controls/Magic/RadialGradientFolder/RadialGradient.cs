using Petty.Views.Controls.Magic.GradientFolder;

namespace Petty.Views.Controls.Magic.RadialGradientFolder;

public class RadialGradient : Gradient
{
    public RadialGradient()
    {
        _renderer = new RadialGradientRenderer(this);
    }

    private readonly RadialGradientRenderer _renderer;

    public static readonly BindableProperty CenterProperty = BindableProperty.Create("Center", typeof(Point), typeof(RadialGradient), default(Point));

    public static readonly BindableProperty RadiusXProperty = BindableProperty.Create("RadiusXProperty", typeof(float), typeof(RadialGradient), -1f);

    public static readonly BindableProperty RadiusYProperty = BindableProperty.Create("RadiusYProperty", typeof(float), typeof(RadialGradient), -1f);

    public static readonly BindableProperty FlagsProperty = BindableProperty.Create("Flags", typeof(RadialGradientFlags), typeof(RadialGradient), RadialGradientFlags.PositionProportional);

    public static readonly BindableProperty ShapeProperty = BindableProperty.Create("Shape", typeof(RadialGradientShape), typeof(RadialGradient), RadialGradientShape.Ellipse);

    public static readonly BindableProperty SizeProperty = BindableProperty.Create("Size", typeof(RadialGradientSize), typeof(RadialGradient), RadialGradientSize.FarthestCorner);

    public Point Center { get => (Point)GetValue(CenterProperty); set => SetValue(CenterProperty, value); }
    public float RadiusX { get => (float)GetValue(RadiusXProperty); set => SetValue(RadiusXProperty, value); }
    public float RadiusY { get => (float)GetValue(RadiusYProperty); set => SetValue(RadiusYProperty, value); }
    public RadialGradientFlags Flags { get => (RadialGradientFlags)GetValue(FlagsProperty); set => SetValue(FlagsProperty, value); }
    public RadialGradientShape Shape { get => (RadialGradientShape)GetValue(ShapeProperty); set => SetValue(ShapeProperty, value); }
    public RadialGradientSize Size { get => (RadialGradientSize)GetValue(SizeProperty); set => SetValue(SizeProperty, value); }

    public override void Render(RenderContext context) => _renderer.Render(context);
}