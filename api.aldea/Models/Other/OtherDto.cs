using System;
using System.Collections.Generic;
using api.premier.Models.Catalogos;
using api.premier.Models.ServiceOrderService;

namespace api.premier.Models.Other
{
    public class OtherDto
    {
        public int Id { get; set; }
        public int? WorkOrderServices { get; set; }
        public bool? Coordination { get; set; }
        public DateTime? AuthoDate { get; set; }
        public DateTime? AuthoAcceptanceDate { get; set; }
        public int? DeliverTo { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public string Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        //
        // public virtual CatStatusDto Status { get; set; }
        // public virtual WorkOrderServiceDto WorkOrderServicesNavigation { get; set; }
        public virtual ICollection<CommentOtherDto> CommentOthers { get; set; }
        public virtual ICollection<DocumentOtherDto> DocumentOthers { get; set; }
        public virtual ICollection<ReminderOtherDto> ReminderOthers { get; set; }
    }
}