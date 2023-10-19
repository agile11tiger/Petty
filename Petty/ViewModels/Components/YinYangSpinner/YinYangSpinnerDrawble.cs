namespace Petty.ViewModels.Components.YinYangSpinner;

/// <summary>
/// It's fun, there are a lot of pictures and text, but I didn't read it :).
/// Carefully, there are 3 parts.  https://www.daniweb.com/programming/mobile-development/tutorials/538946/android-native-animate-alternating-yin-yang-symbol-part-3
/// https://stackoverflow.com/questions/27010594/converting-a-yin-yang-in-xaml-into-c-sharp
/// css https://codepen.io/ste-vg/pen/oZJmwd
/// some tutorial https://css-tricks.com/creating-yin-yang-loaders-web/
/// but i took this and made same in C# https://codepen.io/thebabydino/pen/NpoxEe/
/// </summary>
public class YinYangSpinnerDrawble : IDrawable
{
    private float _rotate;
    private float _speed;
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.BlendMode = BlendMode.SourceAtop;
        var radius = dirtyRect.Width / 2;
        var rmin = 0.25f * radius;
        var rmax = radius - rmin;
        var magicValue = 0.5 * (1 + Math.Cos(_speed += 0.01f));
        var blackArc = (float)(magicValue * rmin + (1 - magicValue) * rmax);
        var whiteArc = radius - blackArc;

        canvas.Rotate(_rotate += 1, radius, radius);
        canvas.FillColor = Colors.White;
        canvas.FillCircle(radius, radius, radius);                                        //white half (main large white circle)

        canvas.FillColor = Colors.Black;
        canvas.FillArc(0, 0, dirtyRect.Width, dirtyRect.Height, 0, 180, true);            //black half
        canvas.FillArc(0, whiteArc, blackArc * 2, blackArc * 2, 0, 180, false);           //black arc

        canvas.FillColor = Colors.White;
        canvas.FillArc(blackArc * 2, blackArc, whiteArc * 2, whiteArc * 2, 0, 180, true); //white arc
        canvas.FillCircle(blackArc, whiteArc + blackArc, blackArc / 3);                   //white circle

        canvas.FillColor = Colors.Black;
        canvas.FillCircle(whiteArc + blackArc * 2, whiteArc + blackArc, whiteArc / 3);    //black circle
    }
}
