using System;

namespace api.premier.Models.HomeSale
{
    public class ReminderHomeSaleDto
    {
        public int Id { get; set; }
        public int HomeSale { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}