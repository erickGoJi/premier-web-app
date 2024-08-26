using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RentalFurnitureCoordination
{
    public class ReminderRentalFurnitureCoordinationDto
    {
        public int Id { get; set; }
        public int? RentalFurnitureCoordinationId { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual RentalFurnitureCoordinationDto RentalFurnitureCoordination { get; set; }
    }
}
