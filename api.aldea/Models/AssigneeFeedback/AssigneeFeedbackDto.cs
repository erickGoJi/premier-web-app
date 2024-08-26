using api.premier.Models.AssigneeInformation;
using api.premier.Models.ServiceOrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AssigneeFeedback
{
    public class AssigneeFeedbackDto
    {
        public int Id { get; set; }
        public int? ServiceOrderServicesId { get; set; }
        public int? AssigneeId { get; set; }
        public int? Rating { get; set; }
        public string Feedback { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AssigneeInformationDto Assignee { get; set; }
        public virtual WorkOrderServiceDto ServiceOrderServices { get; set; }
    }
}
