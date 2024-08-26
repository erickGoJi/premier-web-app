using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.TemporaryHousingCoordinaton
{
    public class ReminderTemporaryHousingCoordinatonDto
    {
        public int Id { get; set; }
        public int? TemporaryHousingCoordinationId { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TemporaryHousingCoordinatonDto TemporaryHousingCoordination { get; set; }
    }
}
