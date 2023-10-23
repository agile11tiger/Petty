using Microsoft.Maui.Graphics.Converters;
using Petty.Views.Controls.Magic.CssParser;
using Petty.Views.Controls.Magic.GradientFolder;
namespace Petty.Views.Controls.Magic.ColorFolder;

public class ColorNameDefinition : ITokenDefinition
{
    protected ColorTypeConverter ColorConverter { get; } = new ColorTypeConverter();

    public bool IsMatch(string token)
    {
        var array = token.Split(['.']);

        if (array.Length != 1)
            return array.Length == 2 && array[0] == "Color";

        return true;
    }

    public void Parse(CssReader reader, GradientBuilder builder)
    {
        var array = reader.Read().Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var color = (Color)ColorConverter.ConvertFromInvariantString(array[0]);

        if (array.TryConvertOffsets(out var result))
            builder.AddStops(color, result);
        else
            builder.AddStop(color);
    }
}
