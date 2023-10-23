namespace Petty.Views.Controls.Magic;

internal struct Vector2
{
    private Vector2(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; private set; }
    public double Y { get; private set; }
    public static Vector2 Zero { get; } = new Vector2(0.0, 0.0);
    public static Vector2 Left { get; } = new Vector2(-1.0, 0.0);
    public static Vector2 Right { get; } = new Vector2(1.0, 0.0);
    public static Vector2 Up { get; } = new Vector2(0.0, -1.0);
    public static Vector2 Down { get; } = new Vector2(0.0, 1.0);

    public void SetNamedDirection(string direction)
    {
        switch (direction)
        {
            case "left":
                X = -1.0;
                break;
            case "right":
                X = 1.0;
                break;
            case "top":
                Y = -1.0;
                break;
            case "bottom":
                Y = 1.0;
                break;
            case "center":
                X = 0.0;
                Y = 0.0;
                break;
            default:
                throw new ArgumentOutOfRangeException("Unrecognized direction: '" + direction + "'");
        }
    }

    public static double Angle(ref Vector2 value1, ref Vector2 value2)
    {
        var num = value1.X * value2.X + value1.Y * value2.Y;
        var num2 = Math.Pow(value1.X, 2.0) + Math.Pow(value1.Y, 2.0);
        var num3 = Math.Pow(value2.X, 2.0) + Math.Pow(value2.Y, 2.0);
        var num4 = Math.Sqrt(num2 * num3);
        return Math.Acos(num / num4) * (180.0 / Math.PI);
    }
}