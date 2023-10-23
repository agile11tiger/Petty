using SkiaSharp;
using SkiaSharp.Views.Maui;
namespace Petty.Views.Controls.Magic.RadialGradientFolder;

public class RadialGradientRenderer(RadialGradient _gradient)
{
    public void Render(RenderContext context)
    {
        var info = context.Info;
        var source = _gradient.Stops.OrderBy((x) => x.Offset).ToArray();
        var lastOffset = !_gradient.IsRepeating ? 1f : source.LastOrDefault()?.Offset ?? 1f;
        var colors = source.Select((x) => x.Color.ToSKColor()).ToArray();
        var colorPos = source.Select((x) => x.Offset / lastOffset).ToArray();
        var center = GetCenter(info.Width, info.Height);
        var radius = GetRadius(center, info, lastOffset);
        var item = radius.Item1;
        var item2 = radius.Item2;
        var shader = SKShader.CreateRadialGradient(center, Math.Min(item, item2), colors, colorPos, _gradient.IsRepeating ? SKShaderTileMode.Repeat : SKShaderTileMode.Clamp, GetScaleMatrix(center, item, item2));
        context.Paint.Shader = shader;
        context.Canvas.DrawRect(info.Rect, context.Paint);
    }

    private SKPoint GetCenter(int width, int height)
    {
        var sKPoint = _gradient.Center.ToSKPoint();
        var num = IsProportional(RadialGradientFlags.XProportional);
        return new SKPoint(y: IsProportional(RadialGradientFlags.YProportional) ? height * sKPoint.Y : sKPoint.Y, x: num ? width * sKPoint.X : sKPoint.X);
    }

    private (float, float) GetRadius(SKPoint center, SKImageInfo info, float offset)
    {
        var num = 0f;
        var num2 = 0f;

        if (_gradient.Shape == RadialGradientShape.Ellipse)
        {
            var distanceInPoints = GetDistanceInPoints(center, info);
            var source = from p in distanceInPoints
                         select Math.Abs(p.X) into x
                         where x > 0f
                         select x;
            var source2 = from p in distanceInPoints
                          select Math.Abs(p.Y) into y
                          where y > 0f
                          select y;
            num = _gradient.Size.IsClosest() ? source.Min() : source.Max();
            num2 = _gradient.Size.IsClosest() ? source2.Min() : source2.Max();
        }

        if (_gradient.Shape == RadialGradientShape.Circle)
        {
            var euclideanDistance = GetEuclideanDistance(center, info);
            num2 = num = _gradient.Size.IsClosest() ? euclideanDistance.Min() : euclideanDistance.Max();
        }

        if (_gradient.RadiusX > -1f)
            num = IsProportional(RadialGradientFlags.WidthProportional) ? info.Width * _gradient.RadiusX : _gradient.RadiusX;

        if (_gradient.RadiusY > -1f)
            num2 = IsProportional(RadialGradientFlags.HeightProportional) ? info.Height * _gradient.RadiusY : _gradient.RadiusY;

        return (num * offset, num2 * offset);
    }

    private SKPoint[] GetCornerPoints(SKImageInfo info)
    {
        return [
            new SKPoint(info.Rect.Left, info.Rect.Top),
            new SKPoint(info.Rect.Right, info.Rect.Top),
            new SKPoint(info.Rect.Right, info.Rect.Bottom),
            new SKPoint(info.Rect.Left, info.Rect.Bottom)
        ];
    }

    private SKPoint[] GetSidePoints(SKPoint center, SKImageInfo info)
    {
        return [
                new SKPoint(info.Rect.Left, center.Y),
            new SKPoint(center.X, info.Rect.Top),
            new SKPoint(info.Rect.Right, center.Y),
            new SKPoint(center.X, info.Rect.Bottom)
        ];
    }

    private SKPoint[] GetDistanceInPoints(SKPoint center, SKImageInfo info)
    {
        var array = _gradient.Size.IsCorner() ? GetCornerPoints(info) : GetSidePoints(center, info);
        var array2 = new SKPoint[array.Length];

        for (var i = 0; i < array2.Length; i++)
            array2[i] = center - array[i];

        return array2;
    }

    private float[] GetEuclideanDistance(SKPoint center, SKImageInfo info)
    {
        var array = _gradient.Size.IsCorner() ? GetCornerPoints(info) : GetSidePoints(center, info);
        var array2 = new float[array.Length];

        for (int i = 0; i < array2.Length; i++)
            array2[i] = SKPoint.Distance(center, array[i]);

        return array2;
    }

    private SKMatrix GetScaleMatrix(SKPoint center, float radiusX, float radiusY)
    {
        return radiusX > radiusY ? SKMatrix.MakeScale(radiusX / radiusY, 1f, center.X, center.Y)
            : radiusY > radiusX ? SKMatrix.MakeScale(1f, radiusY / radiusX, center.X, center.Y)
            : SKMatrix.MakeIdentity();
    }

    private bool IsProportional(RadialGradientFlags flag)
    {
        return (_gradient.Flags & flag) != 0;
    }
}