using System.Windows.Media;

namespace Stormbus.UI.Containers
{
    public class StatusBar : NotifyPropertyChanged
    {
        private static readonly SolidColorBrush StoppedColor = new SolidColorBrush(Color.FromRgb(64, 80, 141));
        private static readonly SolidColorBrush WorkingColor = new SolidColorBrush(Color.FromRgb(162, 75, 64));

        public SolidColorBrush StatusBarColor { get; set; } = StoppedColor;

        public void SetWorkingState()
        {
            //StatusBarColor = WorkingColor;
        }

        public void SetStoppedState()
        {
            //StatusBarColor = StoppedColor;
        }
    }
}