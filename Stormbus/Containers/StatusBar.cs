using System.Windows.Media;

namespace Stormbus.UI.Containers
{
    public class StatusBar : NotifyPropertyChanged
    {
        private static readonly SolidColorBrush NormalColor = new SolidColorBrush(Color.FromRgb(64, 80, 141));
        private static readonly SolidColorBrush ErrorColor = new SolidColorBrush(Color.FromRgb(162, 75, 64));

        public SolidColorBrush StatusBarColor { get; set; } = NormalColor;

        public SolidColorBrush StatusBarColorWithTransparency
        {
            get
            {
                var result = StatusBarColor.Clone();
                result.Opacity = 0.8;
                return result;
            }
        }

        public void SetNormalColor()
        {
            //StatusBarColor = WorkingColor;
        }

        public void SetErrorColor()
        {
            //StatusBarColor = StoppedColor;
        }
    }
}