using System;

namespace api.premier.Models.LeaseRenewal
{
    public class ReminderLeaseRenewalDto
    {
        public int Id { get; set; }
        public int LeaseRenewal { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual LeaseRenewalDto LeaseRenewalNavigation { get; set; }
    }
}