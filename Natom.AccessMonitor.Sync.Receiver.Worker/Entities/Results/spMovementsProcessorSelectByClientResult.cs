using Natom.AccessMonitor.Sync.Receiver.Worker.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Worker.Entities.Results
{
    public class spMovementsProcessorSelectByClientResult
    {
        public List<spMovementsProcessorSelectByClientDetailMovementsResult> Movements { get; set; }
        public List<spMovementsProcessorSelectByClientDetailDocketRangesResult> DocketsRanges { get; set; }
        public List<MovementProcessed> LastInOut { get; set; }
    }

    public class spMovementsProcessorSelectByClientDetailDocketRangesResult
    {
        public int DocketRangeId { get; set; }
        public int DocketId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
    }

    public class spMovementsProcessorSelectByClientDetailMovementsResult
    {
        public long MovementId { get; set; }
        public int DeviceId { get; set; }
        public DateTime DateTime { get; set; }
        public string DocketNumber { get; set; }
        public string MovementType { get; set; }
        public int GoalId { get; set; }
        public int PlaceId { get; set; }
        public int DocketId { get; set; }
        public int ExpectedPlaceId { get; set; }
    }
}
