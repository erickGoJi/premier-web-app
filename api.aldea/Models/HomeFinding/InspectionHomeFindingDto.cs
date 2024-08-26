using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class InspectionHomeFindingDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public DateTime InitialInspectionDate { get; set; }
        public DateTime? FinalInspectionDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual HomeFindingDto HomeFinding { get; set; }
    }
}
