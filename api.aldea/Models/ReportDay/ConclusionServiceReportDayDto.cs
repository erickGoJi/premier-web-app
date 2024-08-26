using System;
using biz.premier.Entities;

namespace api.premier.Models.ReportDay
{
    public class ConclusionServiceReportDayDto
    {
        public int Id { get; set; }
        public int ServiceReportDayId { get; set; }
        public string Conclusion { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // public virtual ServiceReportDayDto ServiceReportDay { get; set; }
    }
}