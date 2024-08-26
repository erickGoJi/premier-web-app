using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class ContractDetailDto
    {
        public int ContractDetailId { get; set; }
        public DateTime? LeaseStartDate { get; set; }
        public DateTime? LeaseEndDate { get; set; }
        public string? PaymentsDue { get; set; }
        public decimal? ListRentPrice { get; set; }
        public int? Currency { get; set; }
        public decimal? FinalRentPrice { get; set; }
        public int? CurrencyFinal { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Recurrence { get; set; }
        public int IdContract { get; set; }
        public int? IdServiceDetail { get; set; }
         
        public bool? Visible { get; set; }

        public int? Leasesignatories { get; set; }
        public string Namesinatore { get; set; }
        public string Emailsinatore { get; set; }
        public string Telsinatore { get; set; }

        public int? WorkOrderServicesId { get; set; }
        public int? CatCategoryId { get; set; }
        public string CatCategoryText { get; set; }
        public DateTime? DateCreatedInApp { get; set; }
        public bool? CreatedInApp { get; set; }

        //public virtual HousingListDto ContractDetailNavigation { get; set; }
        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        public virtual ICollection<PropertyExpenseDto> PropertyExpenses { get; set; }
    }
}
