using Petty.Views.Controls.Magic.CssParser;
using System.ComponentModel;
namespace Petty.Views.Controls.Magic.GradientFolder;

[TypeConverter(typeof(CssGradientSourceTypeConverter))]
public interface IGradientSource
{
    IEnumerable<Gradient> GetGradients();
}
