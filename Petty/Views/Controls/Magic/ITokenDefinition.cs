using Petty.Views.Controls.Magic.CssParser;
using Petty.Views.Controls.Magic.GradientFolder;
namespace Petty.Views.Controls.Magic;

public interface ITokenDefinition
{
    bool IsMatch(string token);
    void Parse(CssReader reader, GradientBuilder builder);
}
