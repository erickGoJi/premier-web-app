using System;

namespace api.premier.Models.TenancyManagement
{
    public class ReminderTenancyManagementDto
    {
        public int Id { get; set; }
        public int TenancyManagementId { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Remi { get; set; }
    }
}