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
using Stormbus.UI.Command.CommandData;
using Stormbus.UI.Configuration;
using Stormbus.UI.Containers;
using Stormbus.UI.Enums;
using Stormbus.UI.Logger;

namespace Stormbus.UI.Modbus
{
    public class ModbusClient
    {
        #region Private fields

        private readonly ResultData _resultData;
        private readonly ConfigurationSettingsModel _configurationSettings;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly WorkingState _workingState;
        private IModbusMaster _modbusMaster;
        private TcpClient _tcpClient;
        private UdpClient _udpClient;
        private SerialPort _serialPort;

        #endregion

        #region Constructor

        public ModbusClient(ResultData resultData,
            ConfigurationSettingsModel configurationSettings, WorkingState workingState)
        {
            _resultData = resultData;
            _configurationSettings = configurationSettings;
            _workingState = workingState;
        }

        #endregion

        #region CreateMaster

        /// <summary>
        /// Return the modbus muster depending on the settings
        /// </summary>
        private IModbusMaster CreateMaster()
        {
            var factory =
                new ModbusFactory(logger: new CustomModbusLogger(_configurationSettings.LoggingPanelSettings));
            IModbusMaster master = null;
            if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.TcpIp ||
                _configurationSettings.ConnectionType.CurrentType == ConnectionType.RtuAsciiOverTcpIp)
            {
                master = CreateTcpMaster(factory);
            }

            if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.Serial)
            {
                master = CreateSerialPortMaster(factory);
            }

            if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.UdpIp ||
                _configurationSettings.ConnectionType.CurrentType == ConnectionType.RtuAsciiOverUdpIp)
            {
                master = CreateUdpMaster(factory);
            }

            return master;
        }

        #region CreateMasterByConnectionType

        private IModbusMaster CreateTcpMaster(ModbusFactory factory)
        {
            _tcpClient = new TcpClient(_configurationSettings.IpAddress, _configurationSettings.IpPort);
            IModbusMaster master = null;
            if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.TcpIp)
                master = factory.CreateMaster(_tcpClient);
            if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.RtuAsciiOverTcpIp)
            {
                var adapter = new TcpClientAdapter(_tcpClient);
                if (_configurationSettings.UseRtu) master = factory.CreateRtuMaster(adapter);
                if (_configurationSettings.UseAscii) master = factory.CreateAsciiMaster(adapter);
            }

            master.Transport.ReadTimeout = _configurationSettings.Timeout;
            return master;
        }

        private IModbusMaster CreateUdpMaster(ModbusFactory factory)
        {
            _udpClient = new UdpClient(_configurationSettings.IpAddress, _configurationSettings.IpPort);
            IModbusMaster master = null;
            if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.UdpIp)
                master = factory.CreateMaster(_udpClient);
            if (_configurationSettings.ConnectionType.CurrentType == ConnectionType.RtuAsciiOverUdpIp)
            {
                var adapter = new UdpClientAdapter(_udpClient);
                if (_configurationSettings.UseRtu) master = factory.CreateRtuMaster(adapter);
                if (_configurationSettings.UseAscii) master = factory.CreateAsciiMaster(adapter);
            }

            master.Transport.ReadTimeout = _configurationSettings.Timeout;

            return master;
        }

        private IModbusMaster CreateSerialPortMaster(ModbusFactory factory)
        {
            IModbusMaster master = null;
            _serialPort = new SerialPort
            {
                PortName = _configurationSettings.SerialPort,
                BaudRate = _configurationSettings.BaudRate.Current,
                DataBits = _configurationSettings.DataBits,
                Parity = _configurationSettings.Parity,
                StopBits = _configurationSettings.StopBits
            };
            _serialPort.Open();
            if (_configurationSettings.UseRtu)
            {
                master = factory.CreateRtuMaster(new SerialPortAdapter(_serialPort));
            }

            if (_configurationSettings.UseAscii)
            {
                master = factory.CreateAsciiMaster(new SerialPortAdapter(_serialPort));
            }

            master.Transport.ReadTimeout = _configurationSettings.Timeout;

            return master;
        }

        #endregion

        #endregion

        #region Read

        /// <summary>
        ///     Creates master and starts read asynchronously 
        /// </summary>
        public async void StartReadAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    ResetCancellationToken();
                    var master = CreateMaster();
                    Read(master);
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
                ReleaseConnection();
            }
        }
        
        /// <summary>
        ///     Stops reading
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private void Read(IModbusMaster master)
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
                        ReadByFunction(master, isFirstRead);
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

        private void ReadByFunction(IModbusMaster master, bool isFirstRead)
        {
            IEnumerable result;
            switch (_configurationSettings.Function)
            {
                case 1:
                    result = master.ReadCoils(_configurationSettings.SlaveId,
                        _configurationSettings.StartAddress, _configurationSettings.Count).Select(Convert.ToUInt16);
                    UpdateUI((IEnumerable<bool>)result, isFirstRead);
                    break;
                case 2:
                    result = master.ReadInputs(_configurationSettings.SlaveId,
                        _configurationSettings.StartAddress, _configurationSettings.Count).Select(Convert.ToUInt16);
                    UpdateUI((IEnumerable<bool>)result, isFirstRead);
                    break;
                case 3:
                    result = master.ReadHoldingRegisters(_configurationSettings.SlaveId,
                        _configurationSettings.StartAddress, _configurationSettings.Count);
                    UpdateUI((IEnumerable<ushort>)result, isFirstRead);
                    break;
                case 4:
                    result = master.ReadInputRegisters(_configurationSettings.SlaveId,
                        _configurationSettings.StartAddress, _configurationSettings.Count);
                    UpdateUI((IEnumerable<ushort>)result, isFirstRead);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        #endregion

        #region Command

        /// <summary>
        ///     Execution of commands depending on the input data
        /// </summary>
        /// <param name="commandData">Data that differ depending on the type of command</param>
        public async void ExecuteCommandAsync(CommandDataBase commandData)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (_modbusMaster == null)
                    {
                        try
                        {
                            _modbusMaster = CreateMaster();
                            ExecuteByCommand(_modbusMaster, commandData);
                        }
                        finally
                        {
                            ReleaseConnection();
                        }
                    }
                    else
                    {
                        ExecuteByCommand(_modbusMaster, commandData);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ExecuteByCommand(IModbusMaster master, CommandDataBase commandData)
        {
            if (commandData is SingleCoilCommandData singleCoilCommand)
            {
                master.WriteSingleCoil(_configurationSettings.SlaveId, singleCoilCommand.Address, singleCoilCommand.Value);
            }

            if (commandData is SingleRegisterCommandData singleRegisterCommand)
            {
                master.WriteSingleRegister(_configurationSettings.SlaveId, singleRegisterCommand.Address, singleRegisterCommand.Value);
            }

            if (commandData is MultipleCoilCommandData multipleCoilCommand)
            {
                master.WriteMultipleCoils(_configurationSettings.SlaveId, multipleCoilCommand.Address, multipleCoilCommand.Values);
            }

            if (commandData is MultipleRegisterCommandData multipleRegisterCommand)
            {
                master.WriteMultipleRegisters(_configurationSettings.SlaveId, multipleRegisterCommand.Address, multipleRegisterCommand.Values);
            }
        }

        #endregion

        #region UpdateUI

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

        #endregion

        #region OtherMethods

        private void ResetCancellationToken()
        {
            _cancellationTokenSource = new CancellationTokenSource(); // reset cancellation token status
        }

        private void ReleaseConnection()
        {
            _modbusMaster?.Dispose();
            _modbusMaster = null;
            _tcpClient?.Dispose();
            _tcpClient = null;
            _udpClient?.Dispose();
            _udpClient = null;
            _serialPort?.Dispose();
            _serialPort = null;
        }

        #endregion
    }
}