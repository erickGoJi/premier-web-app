using System;

namespace api.premier.Models.ReportDay
{
    public class ReportDayByIdDto
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
    }
}
