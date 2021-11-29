using Anviz.SDK;
using Natom.AccessMonitor.Sync.Transmitter.Entities;
using Natom.AccessMonitor.Sync.Transmitter.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.DeviceWrappers
{
    public class AnvizDeviceWrapper : IDeviceWrapper
    {
        private DeviceConfig _device;
        private AnvizManager _manager;
        private AnvizConnectionWrapper _connection;

        public AnvizDeviceWrapper(DeviceConfig myDevice)
        {
            _device = myDevice;
            _manager = new AnvizManager();
            _connection = null;
        }

        public void Disconnect()
        {
            if (_connection.DeviceSocket.Connected)
                _connection.DeviceSocket.Close();
        }

        public async Task<List<MovementDto>> GetMovementsAsync(bool onlyNew, CancellationToken cancellationToken)
        {
            if (_connection == null || !_manager.IsConnected(_connection.DeviceSocket))
                _connection = await _manager.Connect(_device.DeviceHost, (int)_device.DevicePort);

            cancellationToken.ThrowIfCancellationRequested();

            var records = await _connection.AnvizDevice.DownloadRecords(onlyNew);

            if (onlyNew && records.Count > 0)
                await _connection.AnvizDevice.ClearNewRecords(amount: (ulong)records.Count);    //MARCAMOS LOS REGISTROS COMO QUE DEJARON DE SER NUEVOS PARA QUE NO LOS TRAIGA EN LA PROXIMA LECTURA!

            var movements = records.Select(record => new MovementDto
            {
                DocketNumber = (long)record.UserCode,
                DateTime = record.DateTime,
                MovementType = record.RecordType == 129 ? "O" : "I"
            }).ToList();

            return movements;
        }

        public async Task SendRebootSignalAsync()
        {
            //LO COMENTO PORQUE NO HACE MAS QUE TRABAR TODO!! OCULTO EL BOTON EN EL frmRelojes PARA NO USAR ESTA FUNCIONALIDAD
            //if (_connection == null || !_manager.IsConnected(_connection.DeviceSocket))
            //    _connection = await _manager.Connect(_device.DeviceHost, (int)_device.DevicePort);

            //await _connection.AnvizDevice.RebootDevice();
        }

        public async Task<bool> TryConnectionAsync(bool raiseException = false)
        {
            bool isValidConnection = false;

            try
            {
                _manager.ConnectionUser = _device.User;
                _manager.ConnectionPassword = _device.Password;
                _manager.AuthenticateConnection = _device.AuthenticateConnection;

                if (_connection == null || !_manager.IsConnected(_connection.DeviceSocket))
                    _connection = await _manager.Connect(_device.DeviceHost, (int)_device.DevicePort);


                if (string.IsNullOrEmpty(_device.DeviceModel))
                {
                    var SN = await _connection.AnvizDevice.GetDeviceSN();
                    var Model = await _connection.AnvizDevice.GetDeviceTypeCode();
                    var basicSettings = await _connection.AnvizDevice.GetBasicSettings();

                    _device.DeviceId = _connection.AnvizDevice.DeviceId;
                    _device.DeviceSerialNumber = SN;
                    _device.DeviceModel = Model;
                    _device.DeviceFirmwareVersion = basicSettings.Firmware;

                    switch (basicSettings.DateFormat)
                    {
                        case Anviz.SDK.Responses.DateFormat.DDMMYY:     _device.DeviceDateTimeFormat = "DDMMYY";    break;
                        case Anviz.SDK.Responses.DateFormat.MMDDYY:     _device.DeviceDateTimeFormat = "MMDDYY";    break;
                        case Anviz.SDK.Responses.DateFormat.YYMMDD:     _device.DeviceDateTimeFormat = "YYMMDD";    break;
                    }
                }

                isValidConnection = true;
            }
            catch (Exception ex)
            {
                if (raiseException)
                    throw ex;
            }

            return isValidConnection;
        }
    }
}
