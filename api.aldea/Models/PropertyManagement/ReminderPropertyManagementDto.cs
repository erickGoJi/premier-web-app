using System;

namespace api.premier.Models.PropertyManagement
{
    public class ReminderPropertyManagementDto
    {
        public int Id { get; set; }
        public int PropertyManagement { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual PropertyManagementDto PropertyManagementNavigation { get; set; }
    }
}