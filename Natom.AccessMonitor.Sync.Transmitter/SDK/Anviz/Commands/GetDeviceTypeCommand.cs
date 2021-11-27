﻿using Anviz.SDK.Commands;
using Anviz.SDK.Utils;
using System.Threading.Tasks;

namespace Anviz.SDK.Commands
{
    class GetDeviceTypeCommand : Command
    {
        private const byte GET_DEVICE_TYPE = 0x48;
        public GetDeviceTypeCommand(ulong deviceId) : base(deviceId)
        {
            BuildPayload(GET_DEVICE_TYPE, new byte[] { });
        }
    }
}

namespace Anviz.SDK
{
    public partial class AnvizDevice
    {
        public async Task<string> GetDeviceTypeCode()
        {
            var response = await DeviceStream.SendCommand(new GetDeviceTypeCommand(DeviceId));
            DeviceId = response.DeviceID;
            return Bytes.GetAsciiString(response.DATA);
        }

        public async Task<BiometricType> GetDeviceBiometricType()
        {
            var code = await GetDeviceTypeCode();
            DeviceBiometricType = BiometricTypes.DecodeBiometricType(code);
            return DeviceBiometricType;
        }
    }
}