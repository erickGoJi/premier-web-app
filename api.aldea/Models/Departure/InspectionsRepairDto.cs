using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Departure
{
    public class InspectionsRepairDto
    {
        public int Id { get; set; }
        public int? DepartureId { get; set; }
        public DateTime InitialInspectionDate { get; set; }
        public DateTime? FinalInspectionDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual DepartureDto Departure { get; set; }
    }
}
