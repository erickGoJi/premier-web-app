using System;
using System.Collections.Generic;
using biz.premier.Entities;

namespace api.premier.Models.HomePurchase
{
    public class HomePurchaseDto
    {
        public int Id { get; set; }
        public int? WorkOrderServices { get; set; }
        public bool? Coordination { get; set; }
        public DateTime? AuthoDate { get; set; }
        public DateTime? AuthoAcceptanceDate { get; set; }
        public int? DeliverTo { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public decimal? PurchaseDesiredPrice { get; set; }
        public int? CurrencyPurchaseDesiredPriceId { get; set; }
        public decimal? PurchaseFinalPrice { get; set; }
        public int? CurrencyPurchaseFinalPriceId { get; set; }
        public int? PurchaseStatusId { get; set; }
        public string Comment { get; set; }
        public bool? RelatedSupplier { get; set; }
        public string Agency { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNo { get; set; }
        public int? PropertyCommission { get; set; }
        public decimal? CommissionAmount { get; set; }
        public int? CurrencyCommissionId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // public virtual CatCurrency CurrencyCommission { get; set; }
        // public virtual CatCurrency CurrencyPurchaseDesiredPrice { get; set; }
        // public virtual CatCurrency CurrencyPurchaseFinalPrice { get; set; }
        // public virtual DependentInformation DeliverToNavigation { get; set; }
        // public virtual CatStatusHomePurchase PurchaseStatus { get; set; }
        // public virtual ConsultantContactsService Realtor { get; set; }
        // public virtual CatStatus Status { get; set; }
        // public virtual SupplierPartnerProfileService SupplierPartner { get; set; }
        // public virtual WorkOrderService WorkOrderServicesNavigation { get; set; }
        public virtual ICollection<CommentHomePurchaseDto> CommentHomePurchases { get; set; }
        public virtual ICollection<DocumentHomePurchaseDto> DocumentHomePurchases { get; set; }
        public virtual ICollection<ReminderHomePurchaseDto> ReminderHomePurchases { get; set; }
    }
}