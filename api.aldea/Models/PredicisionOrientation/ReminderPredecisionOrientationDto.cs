using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PredicisionOrientation
{
    public class ReminderPredecisionOrientationDto
    {
        public int Id { get; set; }
        public int? PredecisionOrientationId { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual PredecisionOrientationDto PredecisionOrientation { get; set; }
    }
}
