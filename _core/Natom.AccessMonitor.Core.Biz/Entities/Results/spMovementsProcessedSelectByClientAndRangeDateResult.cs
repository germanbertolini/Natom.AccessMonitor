using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Entities.Results
{
	[Keyless]
    public class spMovementsProcessedSelectByClientAndRangeDateResult
    {
		public DateTime Date { get; set; }

		public string DocketNumber { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string DNI { get; set; }
		public string Title { get; set; }

		public string ExpectedPlace { get; set; }

		public TimeSpan ExpectedIn { get; set; }
		public DateTime? In { get; set; }
		public string PlaceIn { get; set; }
		public string GoalIn { get; set; }

		public TimeSpan ExpectedOut { get; set; }
		public DateTime? Out { get; set; }
		public string PlaceOut { get; set; }
		public string GoalOut { get; set; }
		public bool? OutWasEstimated { get; set; }

		public TimeSpan? PermanenceTime { get; set; }

		[NotMapped]
		public DateTime ExpectedInDateTime
        {
			get
            {
				return new DateTime(Date.Year, Date.Month, Date.Day, ExpectedIn.Hours, ExpectedIn.Minutes, ExpectedIn.Seconds);
			}
        }

		[NotMapped]
		public DateTime ExpectedOutDateTime
		{
			get
			{
				var date = Date;
				if (ExpectedIn > ExpectedOut)
					date = date.AddDays(1);

				return new DateTime(date.Year, date.Month, date.Day, ExpectedOut.Hours, ExpectedOut.Minutes, ExpectedOut.Seconds);
			}
		}

		[NotMapped]
		public TimeSpan ExpectedPermanenceTime
        {
			get
            {
				if (ExpectedIn > ExpectedOut)
                {
					return ((new TimeSpan(1, 0, 0, 0)) - ExpectedIn) + ExpectedOut;
                }
				else
                {
					return ExpectedOut - ExpectedIn;
                }
            }
        }

	}
}
