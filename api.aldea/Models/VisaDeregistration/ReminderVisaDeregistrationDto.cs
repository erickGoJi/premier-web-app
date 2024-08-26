using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.VisaDeregistration
{
    public class ReminderVisaDeregistrationDto
    {
        public int Id { get; set; }
        public int VisaDeregistrationId { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual VisaDeregistrationDto VisaDeregistration { get; set; }
    }
}
