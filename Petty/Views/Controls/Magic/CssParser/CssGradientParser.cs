using Petty.Views.Controls.Magic.ColorFolder;
using Petty.Views.Controls.Magic.ColorFolderFolder;
using Petty.Views.Controls.Magic.GradientFolder;
using Petty.Views.Controls.Magic.LinearGradientFolder;
using Petty.Views.Controls.Magic.RadialGradientFolder;
namespace Petty.Views.Controls.Magic.CssParser;

public class CssGradientParser
{
    public CssGradientParser()
    {
        _definitions =
        [
            new LinearGradientDefinition(),
            new RadialGradientDefinition(),
            new ColorHexDefinition(),
            new ColorChannelDefinition(),
            new ColorNameDefinition()
        ];
    }

    private readonly ITokenDefinition[] _definitions;

    public Gradient[] ParseCss(string css)
    {
        var gradientBuilder = new GradientBuilder();

        if (string.IsNullOrWhiteSpace(css))
            return gradientBuilder.Build();

        var cssReader = new CssReader(css);

        while (cssReader.CanRead)
        {
            var token = cssReader.Read().Trim();
            _definitions.FirstOrDefault((x) => x.IsMatch(token))?.Parse(cssReader, gradientBuilder);
            cssReader.MoveNext();
        }

        return gradientBuilder.Build().Reverse().ToArray();
    }
}
