using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Departure
{
    public class ExtensionDepartureDto
    {
        public int Id { get; set; }
        public int? DepartureId { get; set; }
        public int? RequestedBy { get; set; }
        public int? AuthorizedBy { get; set; }
        public DateTime? AuthoDate { get; set; }
        public DateTime? AuthoAcceptanceDate { get; set; }
        public int? Time { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual DepartureDto Departure { get; set; }
    }
}
