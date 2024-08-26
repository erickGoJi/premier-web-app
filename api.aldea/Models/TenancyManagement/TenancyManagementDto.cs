using System;
using System.Collections.Generic;
using biz.premier.Models;

namespace api.premier.Models.TenancyManagement
{
    public class TenancyManagementDto
    {
        public int Id { get; set; }
        public int? WorkOrderServices { get; set; }
        public bool? Coordination { get; set; }
        public DateTime? AuthoDate { get; set; }
        public DateTime? AuthoAcceptanceDate { get; set; }
        public int? DeliverTo { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public DateTime? Recurrence { get; set; }
        public DateTime? MonthlyFollowUp { get; set; }
        public int? PropertyId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // public virtual WorkOrderService WorkOrderServicesNavigation { get; set; }
        public virtual ICollection<CommentTenancyManagementDto> CommentTenancyManagements { get; set; }
        public virtual ICollection<DocumentTenancyManagementDto> DocumentTenancyManagements { get; set; }
        public virtual ICollection<ReminderTenancyManagementDto> ReminderTenancyManagements { get; set; }
        public virtual ICollection<ReportAnEventDto> ReportAnEvents { get; set; }
        public virtual List<ReportAnEventTable> EventTables { get; set; }
    }
}