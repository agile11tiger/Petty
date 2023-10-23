using Petty.Views.Controls.Magic.LinearGradientFolder;
using Petty.Views.Controls.Magic.RadialGradientFolder;
namespace Petty.Views.Controls.Magic.GradientFolder;

public class GradientBuilder
{
    private Gradient _lastGradient;
    private readonly List<Gradient> _gradients = [];

    public GradientBuilder AddLinearGradient(double angle, bool isRepeating = false)
    {
        _lastGradient = new LinearGradient
        {
            Angle = angle,
            IsRepeating = isRepeating,
            Stops = new List<GradientStop>()
        };

        _gradients.Add(_lastGradient);
        return this;
    }

    public GradientBuilder AddRadialGradient(Point center, RadialGradientShape shape, RadialGradientSize size, RadialGradientFlags flags = RadialGradientFlags.PositionProportional, bool isRepeating = false)
    {
        _lastGradient = new RadialGradient
        {
            Center = center,
            Shape = shape,
            Size = size,
            Flags = flags,
            IsRepeating = isRepeating,
            Stops = new List<GradientStop>()
        };
        _gradients.Add(_lastGradient);
        return this;
    }

    public GradientBuilder AddStop(Color color, float? offset = null)
    {
        if (_lastGradient == null)
            AddLinearGradient(0.0);

        GradientStop item = new() { Color = color, Offset = offset ?? -1f };
        _lastGradient.Stops.Add(item);
        return this;
    }

    public GradientBuilder AddStops(Color color, IEnumerable<float> offsets)
    {
        foreach (float offset in offsets)
            AddStop(color, offset);

        return this;
    }

    public Gradient[] Build()
    {
        foreach (Gradient gradient in _gradients)
            SetupUndefinedOffsets(gradient);

        return [.. _gradients];
    }

    private void SetupUndefinedOffsets(Gradient gradient)
    {
        var num1 = 0f;
        var num2 = 1f / (gradient.Stops.Count - 1);

        foreach (var stop in gradient.Stops)
        {
            if (stop.Offset < 0f)
                stop.Offset = num1;

            num1 += num2;
        }
    }
}