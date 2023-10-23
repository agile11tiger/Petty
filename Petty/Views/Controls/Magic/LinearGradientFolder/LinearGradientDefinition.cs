using Petty.Views.Controls.Magic.CssParser;
using Petty.Views.Controls.Magic.GradientFolder;
namespace Petty.Views.Controls.Magic.LinearGradientFolder;

public class LinearGradientDefinition : ITokenDefinition
{
    public bool IsMatch(string token)
    {
        if (!(token == "linear-gradient"))
            return token == "repeating-linear-gradient";

        return true;
    }

    public void Parse(CssReader reader, GradientBuilder builder)
    {
        var isRepeating = reader.Read().Trim() == "repeating-linear-gradient";
        var token = reader.ReadNext().Trim();

        if (TryConvertDegreeToAngle(token, out double angle) || TryConvertTurnToAngle(token, out angle) || TryConvertNamedDirectionToAngle(token, out angle))
        {
            builder.AddLinearGradient(angle, isRepeating);
            return;
        }

        builder.AddLinearGradient(0.0, isRepeating);
        reader.Rollback();
    }

    private bool TryConvertDegreeToAngle(string token, out double angle)
    {
        if (token.TryExtractNumber("deg", out var result))
        {
            angle = CssHelpers.FromDegrees(result);
            return true;
        }

        angle = 0.0;
        return false;
    }

    private bool TryConvertTurnToAngle(string token, out double angle)
    {
        if (token.TryExtractNumber("turn", out var result))
        {
            angle = CssHelpers.FromDegrees(360f * result);
            return true;
        }

        angle = 0.0;
        return false;
    }

    private bool TryConvertNamedDirectionToAngle(string token, out double angle)
    {
        CssReader cssReader = new CssReader(token, ' ');

        if (cssReader.CanRead && cssReader.Read() == "to")
        {
            Vector2 value = Vector2.Down;
            Vector2 value2 = Vector2.Zero;
            cssReader.MoveNext();

            while (cssReader.CanRead)
            {
                value2.SetNamedDirection(cssReader.Read());
                cssReader.MoveNext();
            }

            angle = Vector2.Angle(ref value, ref value2);

            if (value2.X > 0.0)
                angle = 360.0 - angle;

            return true;
        }

        angle = 0.0;
        return false;
    }
}
