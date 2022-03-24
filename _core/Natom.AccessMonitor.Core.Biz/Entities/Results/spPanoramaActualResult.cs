using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Entities.Results
{
    public class spPanoramaActualResult
    {
        public int DocketId { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpectedIn { get; set; }
        public DateTime? RealIn { get; set; }
        public DateTime ExpectedOut { get; set; }
        public DateTime? RealOut { get; set; }
    }
}
