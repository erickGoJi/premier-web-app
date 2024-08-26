using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RentalFurnitureCoordination
{
    public class PaymentRentalDto
    {
        public int Id { get; set; }
        public int? IdRentalFurniture { get; set; }
        public int? Responsible { get; set; }
        public string Description { get; set; }
        public int? Currency { get; set; }
        public decimal? Amount { get; set; }

        public virtual CatCurrency CurrencyNavigation { get; set; }
       // public virtual RentalFurnitureCoordination IdRentalFurnitureNavigation { get; set; }
        public virtual CatResponsablePayment ResponsibleNavigation { get; set; }
    }
}
