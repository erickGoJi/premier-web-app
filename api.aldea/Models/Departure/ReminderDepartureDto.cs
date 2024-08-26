using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Departure
{
    public class ReminderDepartureDto
    {
        public int Id { get; set; }
        public int? DepartaureId { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual DepartureDto Departaure { get; set; }
    }
}
