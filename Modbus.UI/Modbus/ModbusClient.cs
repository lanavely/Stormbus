using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
using NModbus.IO;
using NModbus.Serial;
using Stormbus.UI.Configuration;
using Stormbus.UI.Containers;
using Stormbus.UI.Enums;
using Stormbus.UI.Logger;

namespace Stormbus.UI.Modbus
{
    public class ModbusClient
    {
        #region Constructor

        public ModbusClient(ResultData resultData,
            ConfigurationSettingsModel configurationSettings, WorkingState workingState)
        {
            _resultData = resultData;
            _configurationSettings = configurationSettings;
            _workingState = workingState;
        }

        #endregion

        public async void StartReadAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    PrepareStart();
                    CreateMasterAndRead();
                });
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                    Console.WriteLine(e.Message);
            }
            finally
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                    _workingState.CurrentState = WorkingStates.Stopped;
            }
        }

        private void CreateMasterAndRead()
        {
            var factory =
                new ModbusFactory(logger: new CustomModbusLogger(_configurationSettings.LoggingPanelSettings));
            if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.TcpIp ||
                _configurationSettings.ConnectionType.CurrentType == ConnectionType.RtuAsciiOverTcpIp)
            {
                using (var tcpClient = new TcpClient(_configurationSettings.IpAddress, _configurationSettings.IpPort))
                {
                    IModbusMaster master = null;
                    if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.TcpIp)
                        master = factory.CreateMaster(tcpClient);
                    if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.RtuAsciiOverTcpIp)
                    {
                        var adapter = new TcpClientAdapter(tcpClient);
                        if (_configurationSettings.UseRtu) master = factory.CreateRtuMaster(adapter);
                        if (_configurationSettings.UseAscii) master = factory.CreateAsciiMaster(adapter);
                    }

                    master.Transport.ReadTimeout = _configurationSettings.Timeout;
                    ReadWithTimeout(master);
                }

                return;
            }

            if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.Serial)
            {
                using (var serialPort = new SerialPort
                {
                    PortName = _configurationSettings.SerialPort,
                    BaudRate = _configurationSettings.BaudRate.Current,
                    DataBits = _configurationSettings.DataBits,
                    Parity = _configurationSettings.Parity,
                    StopBits = _configurationSettings.StopBits
                })
                {
                    serialPort.Open();
                    if (_configurationSettings.UseRtu)
                    {
                        var master = factory.CreateRtuMaster(new SerialPortAdapter(serialPort));
                        master.Transport.ReadTimeout = _configurationSettings.Timeout;
                        ReadWithTimeout(master);
                        return;
                    }

                    if (_configurationSettings.UseAscii)
                    {
                        var master = factory.CreateAsciiMaster(new SerialPortAdapter(serialPort));
                        master.Transport.ReadTimeout = _configurationSettings.Timeout;
                        ReadWithTimeout(master);
                        return;
                    }
                }

                return;
            }

            if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.UdpIp ||
                _configurationSettings.ConnectionType.CurrentType == ConnectionType.RtuAsciiOverUdpIp)
            {
                using (var udpClient = new UdpClient(_configurationSettings.IpAddress, _configurationSettings.IpPort))
                {
                    IModbusMaster master = null;
                    if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.UdpIp)
                        master = factory.CreateMaster(udpClient);
                    if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.RtuAsciiOverUdpIp)
                    {
                        var adapter = new UdpClientAdapter(udpClient);
                        if (_configurationSettings.UseRtu) master = factory.CreateRtuMaster(adapter);
                        if (_configurationSettings.UseAscii) master = factory.CreateAsciiMaster(adapter);
                    }

                    master.Transport.ReadTimeout = _configurationSettings.Timeout;
                    ReadWithTimeout(master);
                }
            }
        }

        private void ReadWithTimeout(IModbusMaster master)
        {
            var watch = new Stopwatch();
            var isFirstRead = true;
            try
            {
                while (true)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    watch.Start();
                    try
                    {
                        ReadData(master, isFirstRead);
                        isFirstRead = false;
                    }
                    catch (Exception e)
                    {
                        UserLogger.WriteLine(e.Message, _cancellationTokenSource);
                    }

                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (_configurationSettings.Timeout - watch.ElapsedMilliseconds > 0)
                        Thread.Sleep(
                            TimeSpan.FromMilliseconds(
                                _configurationSettings.Timeout - watch.ElapsedMilliseconds));
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    watch.Stop();
                    watch.Reset();
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                UserLogger.WriteLine(e.Message, _cancellationTokenSource);
            }
        }

        private void ReadData(IModbusMaster master, bool isFirstRead)
        {
            IEnumerable result = null;
            switch (_configurationSettings.Function)
            {
                case 1:
                    result = master.ReadCoils(_configurationSettings.SlaveId,
                        _configurationSettings.StartAddress, _configurationSettings.Count).Select(Convert.ToUInt16);
                    UpdateUI((IEnumerable<bool>) result, isFirstRead);
                    break;
                case 2:
                    result = master.ReadInputs(_configurationSettings.SlaveId,
                        _configurationSettings.StartAddress, _configurationSettings.Count).Select(Convert.ToUInt16);
                    UpdateUI((IEnumerable<bool>) result, isFirstRead);
                    break;
                case 3:
                    result = master.ReadHoldingRegisters(_configurationSettings.SlaveId,
                        _configurationSettings.StartAddress, _configurationSettings.Count);
                    UpdateUI((IEnumerable<ushort>) result, isFirstRead);
                    break;
                case 4:
                    result = master.ReadInputRegisters(_configurationSettings.SlaveId,
                        _configurationSettings.StartAddress, _configurationSettings.Count);
                    UpdateUI((IEnumerable<ushort>) result, isFirstRead);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private void UpdateUI(IEnumerable<ushort> inputs, bool isFirstRead)
        {
            if (isFirstRead)
            {
                _resultData.ApplyConfigurationToMain(inputs);
                return;
            }

            _resultData.UpdateDataToMain(inputs);
        }

        private void UpdateUI(IEnumerable<bool> inputs, bool isFirstRead)
        {
            if (isFirstRead)
            {
                _resultData.ApplyConfigurationToMain(inputs);
                return;
            }

            _resultData.UpdateDataToMain(inputs);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        #region private methods

        private void PrepareStart()
        {
            _cancellationTokenSource = new CancellationTokenSource(); // reset cancellation token status
        }

        #endregion

        #region Private fields

        private readonly ResultData _resultData;
        private readonly ConfigurationSettingsModel _configurationSettings;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly WorkingState _workingState;

        #endregion
    }
}