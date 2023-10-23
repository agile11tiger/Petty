using SkiaSharp;
namespace Petty.Views.Controls.Magic;

public class RenderContext(SKCanvas canvas, SKPaint paint, SKImageInfo info)
{
    public SKCanvas Canvas => canvas;
    public SKPaint Paint => paint;
    public SKImageInfo Info => info;
}
