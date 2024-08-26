using System;

namespace api.premier.Models.TenancyManagement
{
    public class CommentReportAnEventDto
    {
        public int Id { get; set; }
        public int ReportAnEventId { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual UserDto CreatedByNavigation { get; set; }
    }
}