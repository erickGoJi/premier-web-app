using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.DocumentManagement
{
    public class ReminderDocumentManagementDto
    {
        public int Id { get; set; }
        public int DocumentManagementId { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
