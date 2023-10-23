using SkiaSharp;
using SkiaSharp.Views.Maui;
namespace Petty.Views.Controls.Magic.LinearGradientFolder;

public class LinearGradientRenderer(LinearGradient gradient)
{
    private readonly LinearGradient _gradient = gradient;

    public void Render(RenderContext context)
    {
        var info = context.Info;
        var source = _gradient.Stops.OrderBy((x) => x.Offset).ToArray();
        var lastOffset = !_gradient.IsRepeating ? 1f : source.LastOrDefault()?.Offset ?? 1f;
        var colors = source.Select((x) => x.Color.ToSKColor()).ToArray();
        var colorPos = source.Select((x) => x.Offset / lastOffset).ToArray();
        var gradientPoints = GetGradientPoints(info.Width, info.Height, _gradient.Angle, lastOffset);
        var item = gradientPoints.Item1;
        var item2 = gradientPoints.Item2;
        var shader = SKShader.CreateLinearGradient(item, item2, colors, colorPos, _gradient.IsRepeating ? SKShaderTileMode.Repeat : SKShaderTileMode.Clamp);
        context.Paint.Shader = shader;
        context.Canvas.DrawRect(context.Info.Rect, context.Paint);
    }

    private (SKPoint, SKPoint) GetGradientPoints(int width, int height, double rotation, float offset)
    {
        var num = rotation / 360.0;
        var num2 = width * Math.Pow(Math.Sin(Math.PI * 2.0 * ((num + 0.75) / 2.0)), 2.0);
        var num3 = height * Math.Pow(Math.Sin(Math.PI * 2.0 * ((num + 0.0) / 2.0)), 2.0);
        var num4 = width * Math.Pow(Math.Sin(Math.PI * 2.0 * ((num + 0.25) / 2.0)), 2.0);
        var num5 = height * Math.Pow(Math.Sin(Math.PI * 2.0 * ((num + 0.5) / 2.0)), 2.0);
        var item = new SKPoint((width - (float)num2) * offset, (float)num3 * offset);
        var item2 = new SKPoint((width - (float)num4) * offset, (float)num5 * offset);
        return (item, item2);
    }
}