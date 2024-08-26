using api.premier.Models.Catalogos;
using api.premier.Models.Departure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PropertyReport
{
    public class RepairDto
    {
        public int Id { get; set; }
        public int? HousingList { get; set; }
        public int? RepairType { get; set; }
        public int? SupplierPartner { get; set; }
        public DateTime? RepairStartDate { get; set; }
        public DateTime? RepairEndDate { get; set; }
        public int? TotalDays { get; set; }
        public decimal? TotalCostRepair { get; set; }
        public int? Currency { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? IdServiceDetail { get; set; }

        public int? GroupIrId { get; set; }

        public int? PaymentResponsibility { get; set; }
        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        //public virtual HousingList HousingListNavigation { get; set; }
        public virtual CatRepairTypeDto RepairTypeNavigation { get; set; }
        public virtual ICollection<DocumentRepairDto> DocumentRepairs { get; set; }
    }
}
