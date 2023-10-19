using System.Globalization;
namespace Petty.Services.Local.Localization;

public class Language
{
    public string Name { get; set; }
    public CultureInfo CultureInfo { get; set; }
}
