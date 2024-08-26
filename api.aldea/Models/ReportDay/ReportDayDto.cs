using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ReportDay
{
    public class ReportDayDto
    {
        public int Id { get; set; }
        public int? ReportNo { get; set; }
        public int? ReportBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ReportDate { get; set; }
        public int? ServiceLine { get; set; }
        public string ServiceLineName { get; set; }
        public int? WorkOrder { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string TotalTime { get; set; }
        public string Activity { get; set; }
        public string Conclusion { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual UserDto ReportByNavigation { get; set; }
        public virtual CatServiceLineDto ServiceLineNavigation { get; set; }
        //public virtual WorkOrderDto WorkOrderNavigation { get; set; }
        public virtual ICollection<ServiceReportDayDto> ServiceReportDays { get; set; }
    }
}
