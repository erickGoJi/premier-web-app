using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Call
{
    public class CallByServiceRecord
    {
        public int Id { get; set; }
        public int ServiceRecordId { get; set; }
        public string Caller { get; set; }
        public string Calle { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public bool Escalate { get; set; }
        public string Duration { get; set; }
        public bool WelcomeCall { get; set; }
        public string Service { get; set; }
        public string ServiceLine { get; set; }
        public string WorkOrder { get; set; }
        public string ServiceId { get; set; }
        public string Comments { get; set; }
    }
}
