using System;
using System.Collections.Generic;

namespace api.premier.Models.TenancyManagement
{
    public class ReportAnEventDto
    {
        public int Id { get; set; }
        public int TenancyManagementId { get; set; }
        public DateTime? EventReportDate { get; set; }
        public DateTime? EventCloseDate { get; set; }
        public int? SeverityId { get; set; }
        public string Description { get; set; }
        public int? StatusId { get; set; }        
        public int? EventId { get; set; }
        public string QuoteApproval { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // public virtual CatStatusReportAnEvent Status { get; set; }
        // public virtual TenancyManagement TenancyManagement { get; set; }
        public virtual ICollection<AssignedPhotoDto> AssignedPhotos { get; set; }
        public virtual ICollection<CommentReportAnEventDto> CommentReportAnEvents { get; set; }
        public virtual ICollection<SupplierConsultantPhotoDto> SupplierConsultantPhotos { get; set; }
    }
}