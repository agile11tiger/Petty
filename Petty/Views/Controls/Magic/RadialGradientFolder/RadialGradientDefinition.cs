using Petty.Views.Controls.Magic.CssParser;
using Petty.Views.Controls.Magic.GradientFolder;

namespace Petty.Views.Controls.Magic.RadialGradientFolder;
public class RadialGradientDefinition : ITokenDefinition
{
    public bool IsMatch(string token) => token == "radial-gradient" || token == "repeating-radial-gradient";

    public void Parse(CssReader reader, GradientBuilder builder)
    {
        var isRepeating = reader.Read().Trim() == "repeating-radial-gradient";
        var reader2 = new CssReader(reader.ReadNext().Trim(), ' ');
        var shape = GetShape(reader2);
        var shapeSize = GetShapeSize(reader2);
        var position = GetPosition(reader2);
        var flags = GetFlags(position);
        builder.AddRadialGradient(position, shape, shapeSize, flags, isRepeating);
    }

    private RadialGradientShape GetShape(CssReader reader)
    {
        if (reader.CanRead && Enum.TryParse<RadialGradientShape>(reader.Read().Trim(), ignoreCase: true, out var result))
        {
            reader.MoveNext();
            return result;
        }

        return RadialGradientShape.Ellipse;
    }

    private RadialGradientSize GetShapeSize(CssReader reader)
    {
        if (reader.CanRead && Enum.TryParse<RadialGradientSize>(reader.Read().Replace("-", "").Trim(), ignoreCase: true, out var result))
        {
            reader.MoveNext();
            return result;
        }

        return RadialGradientSize.FarthestCorner;
    }

    private Point GetPosition(CssReader reader)
    {
        if (reader.CanRead && reader.Read().Trim() == "at")
        {
            var text = reader.ReadNext();
            var text2 = reader.ReadNext();
            var num = text.TryConvertOffset(out var result);
            var flag = text2.TryConvertOffset(out var result2);
            var zero = Vector2.Zero;

            if (!num && !string.IsNullOrEmpty(text))
                zero.SetNamedDirection(text);

            if (!flag && !string.IsNullOrEmpty(text2))
                zero.SetNamedDirection(text2);

            return new Point(num ? (double)result : (zero.X + 1.0) / 2.0, flag ? (double)result2 : (zero.Y + 1.0) / 2.0);
        }

        return new Point(0.5, 0.5);
    }

    private RadialGradientFlags GetFlags(Point position)
    {
        var radialGradientFlags = RadialGradientFlags.None;

        if (position.X <= 1.0)
            radialGradientFlags |= RadialGradientFlags.XProportional;

        if (position.Y <= 1.0)
            radialGradientFlags |= RadialGradientFlags.YProportional;

        return radialGradientFlags;
    }
}