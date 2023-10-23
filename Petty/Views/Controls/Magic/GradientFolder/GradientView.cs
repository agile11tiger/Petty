using Petty.Views.Controls.Magic.GradientFolder;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
namespace Petty.Views.Controls.Magic;

/// <summary>
/// from https://github.com/mgierlasinski/MagicGradients
/// </summary>
public class GradientView : SKCanvasView
{
    public static readonly BindableProperty GradientSourceProperty = BindableProperty.Create(nameof(GradientSource), typeof(IGradientSource), typeof(GradientView), null, BindingMode.OneWay, null, OnGradientSourceChanged);
    public IGradientSource GradientSource { get => (IGradientSource)GetValue(GradientSourceProperty); set => SetValue(GradientSourceProperty, value); }

    private static void OnGradientSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((GradientView)bindable).InvalidateSurface();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);
        var canvas = e.Surface.Canvas;
        canvas.Clear();

        if (GradientSource == null)
            return;

        using var paint = new SKPaint();
        var context = new RenderContext(canvas, paint, e.Info);

        foreach (Gradient gradient in GradientSource.GetGradients())
            gradient.Render(context);
    }
}