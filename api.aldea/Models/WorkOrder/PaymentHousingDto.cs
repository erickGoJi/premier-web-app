using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class PaymentHousingDto
    {
        public int Id { get; set; }
        public int? HousingList { get; set; }
        public int? PaymentType { get; set; }
        public int? Responsible { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Currency { get; set; }
        public decimal? Amount { get; set; }

        public int? IdServiceDetail { get; set; }

        public int? GroupPaymentsHousingId { get; set; }

        public virtual CatPaymentTypeDto PaymentTypeNavigation { get; set; }

    }
}
