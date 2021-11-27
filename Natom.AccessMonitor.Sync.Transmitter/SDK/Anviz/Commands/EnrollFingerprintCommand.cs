﻿using Anviz.SDK.Commands;
using Anviz.SDK.Utils;
using System;
using System.Threading.Tasks;

namespace Anviz.SDK.Commands
{
    class EnrollFingerprintCommand : Command
    {
        private const byte ENROLL_FINGERPRINT = 0x5C;
        public EnrollFingerprintCommand(ulong deviceId, ulong employeeID, bool isFirst) : base(deviceId)
        {
            var payload = new byte[7];
            Bytes.Write(5, employeeID).CopyTo(payload, 0);
            payload[5] = 1;
            payload[6] = (byte)(isFirst ? 0 : 1);
            BuildPayload(ENROLL_FINGERPRINT, payload);
        }
    }
}

namespace Anviz.SDK
{
    public partial class AnvizDevice
    {
        public async Task<byte[]> EnrollFingerprint(ulong employeeID, int verifyCount = 2)
        {
            if (verifyCount < 1)
            {
                throw new ArgumentException("verifyCount should be at least 1");
            }
            var first = true;
            while (verifyCount-- > 0)
            {
                await DeviceStream.SendCommand(new EnrollFingerprintCommand(DeviceId, employeeID, first));
                first = false;
            }
            return await GetFingerprintTemplate(employeeID, 0);
        }
    }
}