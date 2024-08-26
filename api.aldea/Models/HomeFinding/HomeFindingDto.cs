using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class HomeFindingDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? LeaseGuarentee { get; set; }
        public int? LeaseSignator { get; set; }
        public int? LeaseTemplate { get; set; }
        public bool? ClientSecurityInspectionRequired { get; set; }
        public decimal? PurchaseDesiredPrice { get; set; }
        public int? CurrencyPurchaseDesiredPrice { get; set; }
        public decimal? PurchaseFinalPrice { get; set; }
        public int? CurrencyPurchaseFinalPrice { get; set; }
        public int? PurchaseStatus { get; set; }
        public string Comment { get; set; }
        public bool? RelatedSupplier { get; set; }
        public int? SupplierPartner { get; set; }
        public int? Realtor { get; set; }
        public string CommentRelatedSupplier { get; set; }
        public string PropertComission { get; set; }
        public decimal? ComissionAmount { get; set; }
        public int? ComissionCurrency { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? ContractTypeId { get; set; }
        public int? SecurityDepositId { get; set; }
        public int? InitialRentPaymentId { get; set; }
        public int? OngoingRentPaymentId { get; set; }
        public int? RealtorCommissionId { get; set; }
        public string OtherPaymnet { get; set; }

        public string number_server { get; set; }
        public int userId { get; set; }
        public virtual ICollection<CommentHomeFindingDto> CommentHomeFindings { get; set; }
        public virtual ICollection<DocumentHomeFindingDto> DocumentHomeFindings { get; set; }
        public virtual ICollection<CostSavingDto> CostSavings { get; set; }
        public virtual ICollection<ExtensionHomeFindingDto> ExtensionHomeFindings { get; set; }
        public virtual ICollection<ReminderHomeFindingDto> ReminderHomeFindings { get; set; }

        //public virtual CatLeaseTemplate LeaseTemplateNavigation { get; set; }
        //public virtual ICollection<CostSavingDetailDto> CostSavingDetails { get; set; }
        //public virtual ICollection<DepartureDetailDto> DepartureDetails { get; set; }
        //public virtual ICollection<IncludedRentDto> IncludedRents { get; set; }
        //public virtual ICollection<LandLordBankDto> LandLordBanks { get; set; }
        //public virtual ICollection<LandlordDetailDto> LandlordDetails { get; set; }
        //public virtual ICollection<PaymentHomeFindingDto> PaymentHomeFindings { get; set; }
        //public virtual ICollection<PermanentHomeDto> PermanentHomes { get; set; }
        //public virtual ICollection<RenewalDetailDto> RenewalDetails { get; set; }
        //public virtual ICollection<HomeFindingRepairDto> HomeFindingRepairs { get; set; }
        //public virtual ICollection<IncludedRentDto> IncludedRents { get; set; }
        //public virtual ICollection<InspectionHomeFindingDto> InspectionHomeFindings { get; set; }
    }

    public class HomeFindingSelectDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? LeaseGuarentee { get; set; }
        public int? LeaseSignator { get; set; }
        public int? LeaseTemplate { get; set; }
        public bool? ClientSecurityInspectionRequired { get; set; }
        public decimal? PurchaseDesiredPrice { get; set; }
        public int? CurrencyPurchaseDesiredPrice { get; set; }
        public decimal? PurchaseFinalPrice { get; set; }
        public int? CurrencyPurchaseFinalPrice { get; set; }
        public int? PurchaseStatus { get; set; }
        public string Comment { get; set; }
        public bool? RelatedSupplier { get; set; }
        public int? SupplierPartner { get; set; }
        public int? Realtor { get; set; }
        public string CommentRelatedSupplier { get; set; }
        public string PropertComission { get; set; }
        public decimal? ComissionAmount { get; set; }
        public int? ComissionCurrency { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? ContractTypeId { get; set; }
        public int? SecurityDepositId { get; set; }
        public int? InitialRentPaymentId { get; set; }
        public int? OngoingRentPaymentId { get; set; }
        public int? RealtorCommissionId { get; set; }
        public string OtherPaymnet { get; set; }

        public virtual ICollection<CommentHomeFindingSelectDto> CommentHomeFindings { get; set; }
        public virtual ICollection<DocumentHomeFindingDto> DocumentHomeFindings { get; set; }
        public virtual ICollection<CostSavingDto> CostSavings { get; set; }
        public virtual ICollection<ExtensionHomeFindingDto> ExtensionHomeFindings { get; set; }
        public virtual ICollection<ReminderHomeFindingDto> ReminderHomeFindings { get; set; }

        //public virtual CatLeaseTemplate LeaseTemplateNavigation { get; set; }
        //public virtual ICollection<CommentHomeFindingSelectDto> CommentHomeFindings { get; set; }
        //public virtual ICollection<CostSavingDetailDto> CostSavingDetails { get; set; }
        //public virtual ICollection<CostSavingDto> CostSavings { get; set; }
        //public virtual ICollection<DepartureDetailDto> DepartureDetails { get; set; }
        //public virtual ICollection<DocumentHomeFindingSelectDto> DocumentHomeFindings { get; set; }
        //public virtual ICollection<ExtensionHomeFindingDto> ExtensionHomeFindings { get; set; }
        //public virtual ICollection<IncludedRentDto> IncludedRents { get; set; }
        //public virtual ICollection<LandLordBankDto> LandLordBanks { get; set; }
        //public virtual ICollection<LandlordDetailDto> LandlordDetails { get; set; }
        //public virtual ICollection<PaymentHomeFindingDto> PaymentHomeFindings { get; set; }
        //public virtual ICollection<PermanentHomeDto> PermanentHomes { get; set; }
        //public virtual ICollection<ReminderHomeFindingDto> ReminderHomeFindings { get; set; }
        //public virtual ICollection<RenewalDetailDto> RenewalDetails { get; set; }
        //public virtual ICollection<HomeFindingRepairDto> HomeFindingRepairs { get; set; }
        ////public virtual ICollection<IncludedRentDto> IncludedRents { get; set; }
        //public virtual ICollection<InspectionHomeFindingDto> InspectionHomeFindings { get; set; }
    }
}
