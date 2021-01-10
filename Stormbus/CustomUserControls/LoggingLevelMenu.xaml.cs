using System.Windows;
using System.Windows.Controls;
using NModbus;

namespace Stormbus.UI.CustomUserControls
{
    /// <summary>
    /// Interaction logic for LoggingLevelMenu.xaml
    /// </summary>
    public partial class LoggingLevelMenu : UserControl
    {
        public LoggingLevelMenu()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LoggingLevelProperty = DependencyProperty.Register(
            nameof(LoggingLevel), typeof(LoggingLevel), typeof(LoggingLevelMenu), new PropertyMetadata(LoggingLevel.Information));

        public LoggingLevel LoggingLevel
        {
            get => (LoggingLevel) GetValue(LoggingLevelProperty);
            set => SetValue(LoggingLevelProperty, value);
        }

        private void RadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Tag is LoggingLevel loggingLevel)
            {
                LoggingLevel = loggingLevel;
            }
        }
    }
}
