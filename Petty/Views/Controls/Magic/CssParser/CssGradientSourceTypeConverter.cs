using System.ComponentModel;
using System.Globalization;
namespace Petty.Views.Controls.Magic.CssParser;

public class CssGradientSourceTypeConverter : TypeConverter
{
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        var valueAsString = value?.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(valueAsString))
            throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(CssGradientSource)}");

        return new CssGradientSource { Stylesheet = (string)value };
    }
}