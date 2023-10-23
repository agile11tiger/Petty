namespace Petty.Views.Controls.Magic.RadialGradientFolder;

public static class RadialGradientSizeExtensions
{
    public static bool IsSide(this RadialGradientSize size) => (int)size % 2 != 0;
    public static bool IsCorner(this RadialGradientSize size) => (int)size % 2 == 0;
    public static bool IsClosest(this RadialGradientSize size) => size < RadialGradientSize.FarthestSide;
    public static bool IsFarthest(this RadialGradientSize size) => size >= RadialGradientSize.FarthestSide;
}