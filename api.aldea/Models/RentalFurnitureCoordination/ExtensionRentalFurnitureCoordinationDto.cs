using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RentalFurnitureCoordination
{
    public class ExtensionRentalFurnitureCoordinationDto
    {
        public int Id { get; set; }
        public int? RentalFurnitureCoordinationId { get; set; }
        public int? RequestedBy { get; set; }
        public int? AuthorizedBy { get; set; }
        public DateTime? AuthoDate { get; set; }
        public DateTime? AuthoAcceptanceDate { get; set; }
        public int? Time { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual RentalFurnitureCoordinationDto RentalFurnitureCoordination { get; set; }
    }
}
