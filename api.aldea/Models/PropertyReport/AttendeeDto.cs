using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PropertyReport
{
    public class AttendeeDto
    {
        public int Id { get; set; }
        public int? PropertyReport { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? Relationship { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string Att { get; set; }

        public virtual PropertyReportDto PropertyReportNavigation { get; set; }
        //public virtual CatRelationshipDto RelationshipNavigation { get; set; }
    }
}
