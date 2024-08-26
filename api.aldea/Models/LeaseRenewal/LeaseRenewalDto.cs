using System;
using System.Collections.Generic;
using api.premier.Models.Catalogos;
using biz.premier.Entities;

namespace api.premier.Models.LeaseRenewal
{
    public class LeaseRenewalDto
    {
        public int Id { get; set; }
        public int? WorkOrderServices { get; set; }
        public DateTime? AuthoDate { get; set; }
        public DateTime? AuthoAcceptanceDate { get; set; }
        public int? DeliverTo { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? Property { get; set; }
        public int? CreationBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatStatusDto Status { get; set; }
        // public virtual WorkOrderServiceDto WorkOrderServicesNavigation { get; set; }
        public virtual ICollection<CommentLeaseRenewalDto> CommentLeaseRenewals { get; set; }
        public virtual ICollection<DocumentLeaseRenewalDto> DocumentLeaseRenewals { get; set; }
        public virtual ICollection<ReminderLeaseRenewalDto> ReminderLeaseRenewals { get; set; }
    }
}