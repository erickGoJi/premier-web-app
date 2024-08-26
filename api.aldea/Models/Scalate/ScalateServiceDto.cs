using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Scalate
{
    public class ScalateServiceDto
    {
        public int Id { get; set; }
        public int? UserToId { get; set; }
        public int? UserFromId { get; set; }
        public bool? Status { get; set; }
        public int? EscalationLevel { get; set; }
        public int? WorkOrderId { get; set; }
        public int? ServiceId { get; set; }
        public int? ServiceLineId { get; set; }
        public DateTime? Escalation { get; set; }
        public int? ServiceRecordId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public virtual ICollection<ScalateCommentDto> ScalateComments { get; set; }
        public virtual ICollection<ScalateDocumentDto> ScalateDocuments { get; set; }
    }
}
