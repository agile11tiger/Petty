using Microsoft.Maui.Graphics.Converters;
using Petty.Views.Controls.Magic.CssParser;
using Petty.Views.Controls.Magic.GradientFolder;
using System.Text;
namespace Petty.Views.Controls.Magic.ColorFolder;

public class ColorChannelDefinition : ITokenDefinition
{
    private static readonly char[] _separator = [' '];
    protected ColorTypeConverter ColorConverter { get; } = new ColorTypeConverter();

    public bool IsMatch(string token)
    {
        if (!token.Equals("rgb", StringComparison.OrdinalIgnoreCase) && !token.Equals("rgba", StringComparison.OrdinalIgnoreCase) && !token.Equals("hsl", StringComparison.OrdinalIgnoreCase))
            return token.Equals("hsla", StringComparison.OrdinalIgnoreCase);

        return true;
    }

    public void Parse(CssReader reader, GradientBuilder builder)
    {
        var color = (Color)ColorConverter.ConvertFromInvariantString(GetColorString(reader));

        if (reader.ReadNext().Split(_separator, StringSplitOptions.RemoveEmptyEntries).TryConvertOffsets(out var result))
        {
            builder.AddStops(color, result);
            return;
        }

        builder.AddStop(color);
        reader.Rollback();
    }

    private string GetColorString(CssReader reader)
    {
        var text = reader.Read().Trim();
        var stringBuilder = new StringBuilder(text);
        stringBuilder.Append('(');
        stringBuilder.Append(reader.ReadNext());
        stringBuilder.Append(',');
        stringBuilder.Append(reader.ReadNext());
        stringBuilder.Append(',');
        stringBuilder.Append(reader.ReadNext());

        if (text == "rgba" || text == "hsla")
        {
            stringBuilder.Append(',');
            stringBuilder.Append(reader.ReadNext());
        }

        stringBuilder.Append(')');
        return stringBuilder.ToString();
    }
}
