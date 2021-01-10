using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Stormbus.UI.Configuration;
using Stormbus.UI.Converters;
using Stormbus.UI.Enums;
using Stormbus.UI.Extenstion;
using Stormbus.UI.Helper;

namespace Stormbus.UI.Containers
{
    public class ResultData : NotifyPropertyChanged
    {
        private readonly ConfigurationSettingsModel _configurationSettings;
        private bool[] _boolOriginData;
        private ushort[] _registersOriginData;
        private DataGrid dataGrid;

        public ResultData(ConfigurationSettingsModel configurationSettings)
        {
            _configurationSettings = configurationSettings;
            _configurationSettings.DataTypeChanged += DataTypeChanged;
        }

        public ObservableCollection<ResultItemModel> DisplayData { get; } = new ObservableCollection<ResultItemModel>();

        public void ApplyConfiguration(ushort[] data)
        {
            DisplayData.Clear();
            for (var i = _configurationSettings.StartAddress;
                i < _configurationSettings.StartAddress + _configurationSettings.Count;)
            {
                DisplayData.Add(new ResultItemModel {Address = i});

                switch (_configurationSettings.DataType)
                {
                    case DataType.Short:
                    case DataType.UShort:
                        i += 1;
                        break;
                    case DataType.Int:
                    case DataType.UInt:
                    case DataType.Float:
                        i += 2;
                        break;
                    case DataType.Long:
                    case DataType.Double:
                        i += 4;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            UpdateData(data);
        }

        public void ApplyConfiguration(bool[] data)
        {
            DisplayData.Clear();
            DisplayData.AddRange(Enumerable.Range(_configurationSettings.StartAddress, _configurationSettings.Count)
                .Select(i => new ResultItemModel {Address = Convert.ToUInt16(i)}));
            UpdateData(data);
        }

        public void DataTypeChanged()
        {
            if (_configurationSettings.Function == 1 || _configurationSettings.Function == 2)
            {
                if (_boolOriginData == null)
                    return;
                //_boolOriginData = new bool[_configurationSettings.Count];
                ApplyConfiguration(_boolOriginData);
            }
            else
            {
                if (_registersOriginData == null)
                    return;
                //_registersOriginData = new ushort[_configurationSettings.Count];
                ApplyConfiguration(_registersOriginData);
            }
        }

        public void UpdateData(ushort[] data)
        {
            _registersOriginData = data;
            var b = new List<int>();
            using (var originDataEnumerator = ((IEnumerable<ushort>) _registersOriginData).GetEnumerator())
            {
                foreach (var item in DisplayData)
                {
                    object convertedItem = null;
                    var displayString = string.Empty;
                    switch (_configurationSettings.DataType)
                    {
                        case DataType.Short:
                            convertedItem = ModbusDataTypesConverter.ConvertToShort(
                                ComposeToArray(originDataEnumerator, 1), _configurationSettings.RegistersEndian,
                                _configurationSettings.BytesEndian);
                            displayString = Convert.ToString((short) convertedItem);
                            break;
                        case DataType.UShort:
                            convertedItem = ModbusDataTypesConverter.ConvertToUShort(
                                ComposeToArray(originDataEnumerator, 1), _configurationSettings.RegistersEndian,
                                _configurationSettings.BytesEndian);
                            displayString = Convert
                                .ToString((ushort) convertedItem, (int) _configurationSettings.NumberSystem).ToUpper();
                            break;
                        case DataType.Int:
                            convertedItem = ModbusDataTypesConverter.ConvertToInt(
                                ComposeToArray(originDataEnumerator, 2), _configurationSettings.RegistersEndian,
                                _configurationSettings.BytesEndian);
                            displayString = Convert.ToString((int) convertedItem);
                            break;
                        case DataType.UInt:
                            convertedItem = ModbusDataTypesConverter.ConvertToUInt(
                                ComposeToArray(originDataEnumerator, 2), _configurationSettings.RegistersEndian,
                                _configurationSettings.BytesEndian);
                            displayString = Convert
                                .ToString((uint) convertedItem, (int) _configurationSettings.NumberSystem).ToUpper();
                            break;
                        case DataType.Long:
                            convertedItem = ModbusDataTypesConverter.ConvertToLong(
                                ComposeToArray(originDataEnumerator, 4), _configurationSettings.RegistersEndian,
                                _configurationSettings.BytesEndian);
                            displayString = Convert.ToString((long) convertedItem);
                            break;
                        case DataType.Float:
                            convertedItem = ModbusDataTypesConverter.ConvertToFloat(
                                ComposeToArray(originDataEnumerator, 2), _configurationSettings.RegistersEndian,
                                _configurationSettings.BytesEndian);
                            displayString = convertedItem.ToString();
                            break;
                        case DataType.Double:
                            convertedItem = ModbusDataTypesConverter.ConvertToDouble(
                                ComposeToArray(originDataEnumerator, 4), _configurationSettings.RegistersEndian,
                                _configurationSettings.BytesEndian);
                            displayString = convertedItem.ToString();
                            break;
                    }

                    item.Value = convertedItem;
                    item.DisplayValue = displayString;
                }
            }
        }

        public void UpdateData(bool[] data)
        {
            _boolOriginData = data;
            using var originDataEnumerator = ((IEnumerable<bool>) _boolOriginData).GetEnumerator();
            foreach (var item in DisplayData)
                if (originDataEnumerator.MoveNext())
                {
                    item.Value = originDataEnumerator.Current;
                    item.DisplayValue = originDataEnumerator.Current.ToString();
                }
                else
                {
                    item.Value = false;
                    item.DisplayValue = false.ToString();
                }
        }

        private ushort[] ComposeToArray(IEnumerator<ushort> dataEnumerator, int count)
        {
            var a = new ushort[count];
            for (var i = 0; i < count; i++)
                if (dataEnumerator.MoveNext())
                    a[i] = dataEnumerator.Current;

            return a;
        }

        public void ApplyConfigurationToMain(ushort[] data)
        {
            ThreadController.InvokeToMain(() => { ApplyConfiguration(data); });
        }

        public void ApplyConfigurationToMain(bool[] data)
        {
            ThreadController.InvokeToMain(() => { ApplyConfiguration(data); });
        }

        public void UpdateDataToMain(ushort[] data)
        {
            ThreadController.InvokeToMain(() => { UpdateData(data); });
        }

        public void UpdateDataToMain(bool[] data)
        {
            ThreadController.InvokeToMain(() => { UpdateData(data); });
        }
    }
}