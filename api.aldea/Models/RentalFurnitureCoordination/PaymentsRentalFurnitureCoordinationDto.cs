using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RentalFurnitureCoordination
{
    public class PaymentsRentalFurnitureCoordinationDto
    {
        public int Id { get; set; }
        public int? RentalFurnitureCoordinationId { get; set; }
        public string PaymentType { get; set; }
        public string PaymentResponsibility { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual CatPaymentType PaymentTypeNavigation { get; set; }
        public virtual RentalFurnitureCoordinationDto RentalFurnitureCoordination { get; set; }
    }
}
