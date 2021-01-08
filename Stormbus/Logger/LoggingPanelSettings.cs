using NModbus;

namespace Stormbus.UI.Logger
{
    public class LoggingPanelSettings : NotifyPropertyChanged
    {
        public LoggingLevel MinimumLoggingLevel { get; set; } = LoggingLevel.Critical;
    }
}