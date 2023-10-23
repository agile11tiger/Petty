using Microsoft.Maui.Graphics.Converters;
using Petty.Views.Controls.Magic.CssParser;
using Petty.Views.Controls.Magic.GradientFolder;
namespace Petty.Views.Controls.Magic.ColorFolderFolder;

public class ColorHexDefinition : ITokenDefinition
{
    private static readonly char[] _separator = [' '];
    protected ColorTypeConverter ColorConverter { get; } = new ColorTypeConverter();
    public bool IsMatch(string token) => token.StartsWith("#", StringComparison.Ordinal);

    public void Parse(CssReader reader, GradientBuilder builder)
    {
        var array = reader.Read().Split(_separator, StringSplitOptions.RemoveEmptyEntries);
        var color = (Color)ColorConverter.ConvertFromInvariantString(array[0]);

        if (array.TryConvertOffsets(out var result))
            builder.AddStops(color, result);
        else
            builder.AddStop(color);
    }
}