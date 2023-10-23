using System.Globalization;
namespace Petty.Views.Controls.Magic.CssParser;

public static class CssTokenExtensions
{
    public static bool TryExtractNumber(this string token, string unit, out float result)
    {
        if (token.EndsWith(unit) && float.TryParse(token[..token.LastIndexOf(unit, StringComparison.OrdinalIgnoreCase)], NumberStyles.Any, CultureInfo.InvariantCulture, out var result2))
        {
            result = result2;
            return true;
        }

        result = 0f;
        return false;
    }

    public static bool TryConvertOffset(this string token, out float result)
    {
        if (token != null)
        {
            if (token.TryExtractNumber("%", out var result2))
            {
                result = Math.Min(result2 / 100f, 1f);
                return true;
            }

            if (token.TryExtractNumber("px", out result))
                return true;
        }

        result = 0f;
        return false;
    }

    public static bool TryConvertOffsets(this string[] tokens, out float[] result)
    {
        var list = new List<float>();

        for (var i = 0; i < tokens.Length; i++)
            if (tokens[i].TryConvertOffset(out var result2))
                list.Add(result2);

        result = [.. list];
        return result.Length != 0;
    }
}
