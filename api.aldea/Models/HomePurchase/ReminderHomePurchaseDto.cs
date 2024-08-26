using System;

namespace api.premier.Models.HomePurchase
{
    public class ReminderHomePurchaseDto
    {
        public int Id { get; set; }
        public int HomePurchase { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual HomePurchaseDto HomePurchaseNavigation { get; set; }
    }
}