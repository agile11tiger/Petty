using Petty.Services.Platforms.Paths;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Reflection;

namespace Petty.ViewModels.Components.GraphicsViews
{
    /// <summary>
    /// Delete this class, look summary <see cref="SkCanvasExtension"/>
    /// </summary>
    public class YinYangSpinnerWithTextSkiaSharpViewModel : SKCanvasView
    {
        public YinYangSpinnerWithTextSkiaSharpViewModel()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(PathsService.MONOTYPE_CORSIVA_PATH);
            _font = SKTypeface.FromStream(stream);

            if (_font == null)
                throw new FileLoadException($"Failed to load font: {PathsService.MONOTYPE_CORSIVA_PATH}");
        }

        private int _drawCounter;
        private SKCanvas _canvas;
        private SKRect _drawRect;
        private SKImageInfo _info;
        private readonly SKTypeface _font;
        private float _rotationDegrees = 88;
        private readonly double _speedIncreaseDecrease = Math.PI / 1000f;

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            _canvas = e.Surface.Canvas;
            _canvas.Clear(); // clears the canvas for every frame
            _info = e.Info;
            _drawRect = new SKRect(0, 0, _info.Width, _info.Height);
            Draw(_canvas, _drawRect);
        }

        public void Draw(SKCanvas canvas, SKRect dirtyRect)
        {
            _drawCounter += 1;
            var joint = 1f;
            var blurSize = 11f;
            var initialSizeArcs = 0.25f;
            var paddingFromRectBoundForShadow = 71;
            var radius = dirtyRect.Width / 2 - paddingFromRectBoundForShadow;
            var rmin = 0.11f * radius;
            var rmax = radius - rmin;
            var magicValue = initialSizeArcs * (1 + Math.Cos(_speedIncreaseDecrease * _drawCounter));
            var blackArc = (float)(magicValue * rmin + (1 - magicValue) * rmax);
            var whiteArc = radius - blackArc;
            var blackSkPaint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColor.Parse("#111"),// SKColors.Black,
                Style = SKPaintStyle.StrokeAndFill,
                FilterQuality = SKFilterQuality.None
            };
            var whiteSkPaint = blackSkPaint.Clone();
            whiteSkPaint.Color = SKColors.White;
            var shadowSkPaint = blackSkPaint.Clone();
            shadowSkPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Inner, 500);


            canvas.Translate(paddingFromRectBoundForShadow, paddingFromRectBoundForShadow / 3);
            canvas.DrawOval(radius, radius * 2, radius, radius / 4, shadowSkPaint);
            canvas.Save();
            canvas.RotateDegrees(_rotationDegrees += 0.5f, radius, radius);
            whiteSkPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Solid, blurSize);
            canvas.DrawCircle(radius, radius, radius, whiteSkPaint);                //white half (main large white circle)
            whiteSkPaint.MaskFilter = null;
            canvas.FillArc(0, whiteArc, blackArc * 2, blackArc * 2, 0, 180, false, blackSkPaint);           //black arc
            canvas.FillArc(0, 0, radius * 2, radius * 2 - joint, 0, 180, true, blackSkPaint);               //black half
            canvas.FillArc(blackArc * 2, blackArc - joint, whiteArc * 2, whiteArc * 2, 0, 180, true, whiteSkPaint); //white arc
            canvas.DrawCircle(blackArc, whiteArc + blackArc, blackArc / 3, whiteSkPaint);                   //white circle
            canvas.DrawCircle(whiteArc + blackArc * 2, whiteArc + blackArc, whiteArc / 3, blackSkPaint);    //black circle
            canvas.Restore();

            var textSkPaint = new SKPaint
            {
                TextSize = radius / 3.8f,
                Typeface = _font,
                IsAntialias = true,
                Color = SKColors.White,
                BlendMode = SKBlendMode.Difference,
            };

            canvas.DrawText("Memento Mori", new SKPoint(radius * 0.31f, radius * 0.65f), textSkPaint);
            canvas.DrawText("sed", new SKPoint(radius * 0.80f, radius * 1.04f), textSkPaint);
            canvas.DrawText("Memento Vivere", new SKPoint(radius * 0.25f, radius * 1.45f), textSkPaint);

            blackSkPaint.Shader = SKShader.CreateLinearGradient(
                new SKPoint(0, dirtyRect.Top),
                new SKPoint(0, dirtyRect.Bottom),
                new SKColor[] { blackSkPaint.Color.WithAlpha(0), blackSkPaint.Color.WithAlpha(100) },
                SKShaderTileMode.Repeat);

            canvas.DrawCircle(radius, radius, radius, blackSkPaint);
        }
    }

    /// <summary>
    /// took from https://github.com/dotnet/Microsoft.Maui.Graphics/blob/9294a8c8d87db7e08330d2c33174a0b455c01fdc/src/Microsoft.Maui.Graphics.Skia/SkiaCanvas.cs#L446
    /// Delete when this bug is fixed and use <see cref="YinYangSpinnerDrawble"/>. https://github.com/dotnet/maui/issues/14945
    /// </summary>
    public static class SkCanvasExtension
    {
        public static void FillArc(
            this SKCanvas canvas,
            float x,
            float y,
            float width,
            float height,
            float startAngle,
            float endAngle,
            bool clockwise,
            SKPaint paint)
        {
            while (startAngle < 0)
                startAngle += 360;

            while (endAngle < 0)
                endAngle += 360;

            var sweep = GeometryUtil.GetSweep(startAngle, endAngle, clockwise);
            var rect = new SKRect(x, y, x + width, y + height);

            startAngle *= -1;

            if (!clockwise)
                sweep *= -1;

            // todo: delete this after the api is bound
            var platformPath = new SKPath();
            platformPath.AddArc(rect, startAngle, sweep);
            canvas.DrawPath(platformPath, paint);
            platformPath.Dispose();

            // todo: restore this when the api is bound.
            //_canvas.DrawArc (rect, startAngle, sweep, false, CurrentState.FillPaintWithAlpha);
        }
    }
}
