using System;

namespace api.premier.Models.PropertyManagement
{
    public class VisitReportPropertyManagementDto
    {
        public int Id { get; set; }
        public int? PropertyManagement { get; set; }
        public string VisitReport { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual PropertyManagementDto PropertyManagementNavigation { get; set; }
    }
}