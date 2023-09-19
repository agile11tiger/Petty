using Petty.ViewModels.Base;
using Petty.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.ViewModels.Components
{
    /// <summary>
    /// https://medium.com/@jasper76/creating-a-circular-progress-bar-in-net-maui-22f2f30565fc
    /// </summary>
    public partial class CircularProgressBarViewModel : ObservableObject, IDrawable
    {
        public CircularProgressBarViewModel()
        {
        }

        [ObservableProperty] private float _progress = 19;
        [ObservableProperty] private float _size = 39;
        [ObservableProperty] private float _thickness = 3;
        [ObservableProperty] private Color _progressColor = Colors.YellowGreen;
        [ObservableProperty] private Color _progressLeftColor = Colors.LightGoldenrodYellow;
        [ObservableProperty] private Color _textColor = Colors.Black;

        public static readonly BindableProperty ProgressProperty = BindableProperty.Create(nameof(Progress), typeof(int), typeof(CircularProgressBarViewModel));
        public static readonly BindableProperty SizeProperty = BindableProperty.Create(nameof(Size), typeof(int), typeof(CircularProgressBarViewModel));
        public static readonly BindableProperty ThicknessProperty = BindableProperty.Create(nameof(Thickness), typeof(int), typeof(CircularProgressBarViewModel));
        public static readonly BindableProperty ProgressColorProperty = BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(CircularProgressBarViewModel));
        public static readonly BindableProperty ProgressLeftColorProperty = BindableProperty.Create(nameof(ProgressLeftColor), typeof(Color), typeof(CircularProgressBarViewModel));
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CircularProgressBarViewModel));

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var effectiveSize = Size - Thickness;
            var x = Thickness / 2;
            var y = Thickness / 2;

            if (Progress < 0)
                Progress = 0;
            else if (Progress > 100)
                Progress = 100;

            if (Progress < 100)
            {
                var angle = GetAngle(Progress);
                canvas.StrokeColor = ProgressLeftColor;
                canvas.StrokeSize = Thickness;
                canvas.DrawEllipse(x, y, effectiveSize, effectiveSize);

                // Draw arc
                canvas.StrokeColor = ProgressColor;
                canvas.StrokeSize = Thickness;
                canvas.DrawArc(x, y, effectiveSize, effectiveSize, 90, angle, true, false);
            }
            else
            {
                // Draw circle
                canvas.StrokeColor = ProgressColor;
                canvas.StrokeSize = Thickness;
                canvas.DrawEllipse(x, y, effectiveSize, effectiveSize);
            }

            // Make the percentage always the same size in relation to the size of the progress bar
            var fontSize = effectiveSize / 2.86f;
            canvas.FontSize = fontSize;
            canvas.FontColor = TextColor;

            // Vertical text align the text, and we need a correction factor of around 1.15 to have it aligned properly
            // Note: The VerticalAlignment.Center property of the DrawString method seems to have no effect
            var verticalPosition = (Size / 2 - fontSize / 2) * 1.15f;
            canvas.DrawString($"{Progress}%", x, verticalPosition, effectiveSize, effectiveSize / 4, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        private float GetAngle(float progress)
        {
            var factor = 90f / 25f;

            return progress switch
            {
                > 75 => -180 - (progress - 75) * factor,
                > 50 => -90 - (progress - 50) * factor,
                > 25 => 0 - (progress - 25) * factor,
                _ => 90 - progress * factor
            };
        }
    }
}
