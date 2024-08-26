using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ReportDay
{
    public class ServiceReportDayDto
    {
        public int Id { get; set; }
        public int? ReportDayId { get; set; }
        public int? Service { get; set; }
        public string ServiceName { get; set; }
        public int Category { get; set; }
        public string Time { get; set; }
        public int? TimeReminder { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual ReportDayDto ReportDay { get; set; }
    }
}
