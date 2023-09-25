namespace Petty.ViewModels.Components.GraphicsViews
{
    /// <summary>
    /// https://medium.com/@jasper76/creating-a-circular-progress-bar-in-net-maui-22f2f30565fc
    /// </summary>
    public class CircularProgressBarDrawble : IDrawable
    {
        public float Size { get; set; } = 40;
        public float Thickness { get; set; } = 3;
        public float Percentages { get; set; }
        public Color TextColor { get; set; } = Colors.Black;
        public Color Color { get; set; } = Colors.LightGoldenrodYellow;
        public Color PlaceholderColor { get; set; } = Color.FromRgb(94, 216, 12);

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var effectiveSize = Size - Thickness;
            var x = Thickness / 2;
            var y = Thickness / 2;

            if (Percentages < 0)
                Percentages = 0;
            else if (Percentages > 100)
                Percentages = 100;

            if (Percentages < 100)
            {
                var angle = GetAngle(Percentages);
                canvas.StrokeColor = Color;
                canvas.StrokeSize = Thickness;
                canvas.DrawEllipse(x, y, effectiveSize, effectiveSize);

                // Draw arc
                canvas.StrokeColor = PlaceholderColor;
                canvas.StrokeSize = Thickness;
                canvas.DrawArc(x, y, effectiveSize, effectiveSize, 90, angle, true, false);
            }
            else
            {
                // Draw circle
                canvas.StrokeColor = PlaceholderColor;
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
            canvas.DrawString($"{Percentages}%", x, verticalPosition, effectiveSize, effectiveSize / 4, HorizontalAlignment.Center, VerticalAlignment.Center);
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
