using Natom.AccessMonitor.Sync.Transmitter.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.DeviceWrappers
{
    public interface IDeviceWrapper
    {
        Task<bool> TryConnectionAsync(bool raiseException = false);
        Task SendRebootSignalAsync();
        Task<List<MovementDto>> GetMovementsAsync(bool onlyNew, CancellationToken cancellationToken);
        void Disconnect();
    }
}
