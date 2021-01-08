using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using Stormbus.UI.Containers;
using Stormbus.UI.Enums;
using Stormbus.UI.Logger;

namespace Stormbus.UI.Configuration
{
    public class ConfigurationSettingsModel : DependencyObject, INotifyPropertyChanged
    {
        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region DataTypeSettigns

        public DataType DataType { get; set; } = DataType.UShort;
        public EndianType BytesEndian { get; set; } = EndianType.BigEndian;
        public EndianType RegistersEndian { get; set; } = EndianType.BigEndian;
        public NumberSystem NumberSystem { get; set; } = NumberSystem.Decimal;


        public event Action DataTypeChanged;

        private void OnDataTypeChanged()
        {
            DataTypeChanged?.Invoke();
        }

        private void OnBytesEndianChanged()
        {
            DataTypeChanged?.Invoke();
        }

        private void OnRegistersEndianChanged()
        {
            DataTypeChanged?.Invoke();
        }

        private void OnNumberSystemChanged()
        {
            DataTypeChanged?.Invoke();
        }

        #endregion

        #region ModbusSettings

        private ushort _count = 10;
        private ushort _startAddress;

        public byte SlaveId { get; set; } = 1;

        public byte Function { get; set; } = 3;

        [XmlIgnore] public byte[] FunctionTypes { get; } = {1, 2, 3, 4};

        public ushort StartAddress
        {
            get => _startAddress;
            set
            {
                _startAddress = value;
                StartAddressChanged?.Invoke(value);
            }
        }

        public ushort Count
        {
            get => _count;
            set
            {
                if (_count != value) CountChanged?.Invoke(value);
                _count = value;
            }
        }


        public event Action<ushort> StartAddressChanged;

        public event Action<ushort> CountChanged;

        #endregion

        #region ConnectionSettings

        public int Timeout { get; set; } = 1000;

        public uint ScanRate { get; set; } = 1000;

        public string IpAddress { get; set; } = @"127.0.0.1";

        public ushort IpPort { get; set; } = 502;

        public string SerialPort { get; set; } = System.IO.Ports.SerialPort.GetPortNames().FirstOrDefault() ?? @"COM1";

        [XmlIgnore] public string[] SerialPortNames => System.IO.Ports.SerialPort.GetPortNames();

        public ConnectionType ConnectionType { get; set; } = new ConnectionType();

        public BaudRate BaudRate { get; set; } = new BaudRate();

        public byte DataBits { get; set; } = 8;

        [XmlIgnore] public List<byte> SupportedDataBits => new List<byte> {7, 8};

        public LoggingPanelSettings LoggingPanelSettings { get; } = new LoggingPanelSettings();

        public Parity Parity { get; set; } = Parity.None;

        [XmlIgnore]
        public List<Parity> SupportedParities => new List<Parity>
            {Parity.None, Parity.Odd, Parity.Even, Parity.Mark, Parity.Space};

        public StopBits StopBits { get; set; } = StopBits.One;

        [XmlIgnore]
        public List<StopBits> SupportedStopBits => new List<StopBits>
            {StopBits.None, StopBits.One, StopBits.OnePointFive, StopBits.Two};

        public bool UseRtu { get; set; } = true;

        public bool UseAscii { get; set; }

        #endregion

        #region Serialization

        public void Serialize(string filePath)
        {
            try
            {
                using (var file = CreateStreamStreamWriter(filePath))
                {
                    var serializer = new XmlSerializer(typeof(ConfigurationSettingsModel));
                    serializer.Serialize(file, this);
                }
            }
            catch
            {
                // ignored
            }
        }

        public StreamWriter CreateStreamStreamWriter(string filePath)
        {
            if (!Directory.Exists(StormbusDirectory.StormbusDataFolderPath))
                Directory.CreateDirectory(StormbusDirectory.StormbusDataFolderPath);

            return File.CreateText(filePath);
        }

        public static ConfigurationSettingsModel Deserialize(string filePath)
        {
            try
            {
                using (var streamReader = File.OpenText(filePath))
                {
                    var serializer = new XmlSerializer(typeof(ConfigurationSettingsModel));
                    return (ConfigurationSettingsModel) serializer.Deserialize(streamReader);
                }
            }
            catch
            {
                return new ConfigurationSettingsModel();
            }
        }

        #endregion
    }
}