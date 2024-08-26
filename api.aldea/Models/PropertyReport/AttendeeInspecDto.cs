using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PropertyReport
{
    public class AttendeeInspecDto
    {
        public int Id { get; set; }
        public int? Inspection { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? Relationship { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Att { get; set; }

        public virtual InspectionDto InspectionNavigation { get; set; }
    }
}
