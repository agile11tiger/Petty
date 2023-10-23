namespace Petty.Views.Controls.Magic.CssParser;

public static class CssHelpers
{
    public static double FromDegrees(double degrees) => (180.0 + degrees) % 360.0;
}