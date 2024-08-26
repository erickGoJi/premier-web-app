using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.TemporaryHousingCoordinaton
{
    public class StayExtensionTemporaryHousingDto
    {
        public int Id { get; set; }
        public int? TemporaryHousingCoordinationId { get; set; }
        public DateTime? InitialDate { get; set; }
        public DateTime? FinalDate { get; set; }
        public int? ExtraDays { get; set; }
        public int? TotalDays { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TemporaryHousingCoordinatonDto TemporaryHousingCoordination { get; set; }
    }
}
