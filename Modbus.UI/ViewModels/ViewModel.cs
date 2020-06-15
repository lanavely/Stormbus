using System.ComponentModel;
using System.Windows;
using Stormbus.UI.Configuration;
using Stormbus.UI.Containers;
using Stormbus.UI.Enums;
using Stormbus.UI.Modbus;

namespace Stormbus.UI.ViewModels
{
    public class ViewModel : DependencyObject, INotifyPropertyChanged
    {
        public ViewModel()
        {
            StatusBar = new StatusBar();
            WorkingState = new WorkingState(StatusBar);
            ConfigurationSettings = ConfigurationSettingsModel.Deserialize(StormbusDirectory.ConfigurationFilePath);
            ResultData = new ResultData(ConfigurationSettings);
            ModbusClient = new ModbusClient(ResultData, ConfigurationSettings, WorkingState);
        }

        public ModbusClient ModbusClient { get; }

        public ResultData ResultData { get; }

        public ConfigurationSettingsModel ConfigurationSettings { get; }

        public WorkingState WorkingState { get; }

        public StatusBar StatusBar { get; }

        #region INotificationPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Handlers

        public void StartRead()
        {
            WorkingState.CurrentState = WorkingStates.Work;
            ModbusClient.StartReadAsync();
        }

        public void StopRead()
        {
            WorkingState.CurrentState = WorkingStates.Stopped;
            ModbusClient.Stop();
        }

        #endregion
    }
}