using System;

namespace api.premier.Models.Other
{
    public class ReminderOtherDto
    {
        public int Id { get; set; }
        public int Other { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual OtherDto OtherNavigation { get; set; }
    }
}