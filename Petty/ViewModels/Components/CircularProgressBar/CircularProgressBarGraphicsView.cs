namespace Petty.ViewModels.Components.CircularProgressBar
{
    /// <summary>
    /// https://stackoverflow.com/questions/76020765/net-maui-binding-to-graphicsview
    /// </summary>
    public class CircularProgressBarGraphicsView : GraphicsView
    {
        public static readonly BindableProperty PercentagesProperty = BindableProperty.Create(nameof(Percentages), typeof(float), typeof(CircularProgressBarGraphicsView), propertyChanged: PercentagesPropertyChanged);

        public double Percentages
        {
            get => (double)GetValue(PercentagesProperty);
            set => SetValue(PercentagesProperty, value);
        }

        private static void PercentagesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not CircularProgressBarGraphicsView { Drawable: CircularProgressBarDrawble drawable } view)
                return;

            drawable.Percentages = (float)Convert.ToDouble(newValue);
            view.Invalidate();
        }
    }
}
