using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SettlingIn
{
    public class ReminderSettlingInDto
    {
        public int Id { get; set; }
        public int? SettlingInId { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual SettlingInDto SettlingIn { get; set; }
    }
}
