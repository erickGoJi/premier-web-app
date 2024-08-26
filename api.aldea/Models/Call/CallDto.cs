using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Call
{
    public class CallDto
    {
        public int Id { get; set; }
        public int ServiceRecordId { get; set; }
        public int Caller { get; set; }
        public int Calle { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int Duration { get; set; }
        public bool WelcomeCall { get; set; }
        public int WorkOrderId { get; set; }
        public int ServiceId { get; set; }
        public int? ServiceLineId { get; set; }
        public bool? Escalate { get; set; }
        public string Comments { get; set; }
        public virtual ICollection<CallAssistantDto> CallAssistants { get; set; }
    }
}
