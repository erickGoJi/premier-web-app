using System;
using System.Collections.Generic;
using biz.premier.Entities;

namespace api.premier.Models.HomeSale
{
    public class HomeSaleDto
    {
        public int Id { get; set; }
        public int? WorkOrderServices { get; set; }
        public bool? Coordination { get; set; }
        public DateTime? AuthoDate { get; set; }
        public DateTime? AuthoAcceptanceDate { get; set; }
        public int? DeliverTo { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public decimal? ListPrice { get; set; }
        public int? CurrencyListPriceId { get; set; }
        public decimal? PurchaseFinalPrice { get; set; }
        public int? CurrencyFinalPriceId { get; set; }
        public bool? Property { get; set; }
        public int? PropertyId { get; set; }
        public int? SaleStatusId { get; set; }
        public string Comment { get; set; }
        public string Agency { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNo { get; set; }
        public int? PropertyCommission { get; set; }
        public decimal? CommissionAmount { get; set; }
        public int? CurrencyCommissionId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // public virtual WorkOrderService WorkOrderServicesNavigation { get; set; }
        public virtual ICollection<CommentHomeSaleDto> CommentHomeSales { get; set; }
        public virtual ICollection<DocumentHomeSaleDto> DocumentHomeSales { get; set; }
        public virtual ICollection<ReminderHomeSaleDto> ReminderHomeSales { get; set; }
        public virtual ICollection<VisitHomeSaleDto> VisitHomeSales { get; set; }
    }
}