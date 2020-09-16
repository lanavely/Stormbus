using NModbus;

namespace Stormbus.UI.Logger
{
    public class CustomModbusLogger : IModbusLogger
    {
        public CustomModbusLogger(LoggingPanelSettings loggingPanelSettings)
        {
            LoggingPanelSettings = loggingPanelSettings;
        }

        public LoggingPanelSettings LoggingPanelSettings { get; }

        public bool ShouldLog(LoggingLevel level)
        {
            return level >= LoggingPanelSettings.MinimumLoggingLevel;
        }

        /// <summary>
        ///     Log the specified message at the specified level.
        /// </summary>
        public void Log(LoggingLevel level, string message)
        {
            if (ShouldLog(level)) LogCore(level, message);
        }

        private void LogCore(LoggingLevel level, string message)
        {
            UserLogger.WriteLine(message);
        }
    }
}