using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class ReminderHomeFindingDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        //public virtual HomeFindingDto HomeFinding { get; set; }
    }
}
