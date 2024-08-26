using api.premier.Models.Catalogos;
using api.premier.Models.SupplierPartnerProfileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestPayment
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int? RequestPayment { get; set; }
        public string Desciption { get; set; }
        public bool? Urgent { get; set; }
        public int? ServiceRecord { get; set; }
        public int? WorkOrder { get; set; }
        public int? Service { get; set; }
        public int? WorkOrderServices { get; set; }
        public bool? IfSupplierPartner { get; set; }
        public string SupplierName { get; set; }
        public int? SupplierPartner { get; set; }
        public int? Supplier { get; set; }
        public bool? FiscalInvoice { get; set; }
        public int? PaymentMethod { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? PaymentAmount { get; set; }
        public int? CurrencyPaymentAmount { get; set; }
        public string SupplierInvoiceNo { get; set; }
        public DateTime? SupplierInvoiceDate { get; set; }
        public bool? Recurrence { get; set; }
        public bool? SameWithoutFee { get; set; }
        public string ManagementFee { get; set; }
        public int? CurrencyManagementFee { get; set; }
        public string WireFee { get; set; }
        public int? CurrencyWireFee { get; set; }
        public string AdvenceFee { get; set; }
        public int? CurrencyAdvanceFee { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<CommentPaymentConceptDto> CommentPaymentConcepts { get; set; }
        public virtual ICollection<DocumentPaymentConceptDto> DocumentPaymentConcepts { get; set; }
        public virtual ICollection<RecurrencePaymentConceptDto> RecurrencePaymentConcepts { get; set; }
        public virtual ICollection<WireTransferPaymentConceptDto> WireTransferPaymentConcepts { get; set; }
        public virtual ICollection<WireTransferServicePaymentConceptDto> WireTransferServicePaymentConcepts { get; set; }
        public virtual ICollection<ConceptDto> Concepts { get; set; }

    }
    public class PaymentSelectDto
    {
        public int Id { get; set; }
        public int? RequestPayment { get; set; }
        public string Desciption { get; set; }
        public bool? Urgent { get; set; }
        public int? ServiceRecord { get; set; }
        public int? WorkOrder { get; set; }
        public int? Service { get; set; }
        public int? WorkOrderServices { get; set; }
        public bool? IfSupplierPartner { get; set; }
        public string SupplierName { get; set; }
        public int? SupplierPartner { get; set; }
        public int? Supplier { get; set; }
        public bool? FiscalInvoice { get; set; }
        public int? PaymentMethod { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? PaymentAmount { get; set; }
        public int? CurrencyPaymentAmount { get; set; }
        public string SupplierInvoiceNo { get; set; }
        public DateTime? SupplierInvoiceDate { get; set; }
        public bool? Recurrence { get; set; }
        public bool? SameWithoutFee { get; set; }
        public string ManagementFee { get; set; }
        public int? CurrencyManagementFee { get; set; }
        public string WireFee { get; set; }
        public int? CurrencyWireFee { get; set; }
        public string AdvenceFee { get; set; }
        public int? CurrencyAdvanceFee { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatCurrencyDto CurrencyAdvanceFeeNavigation { get; set; }
        public virtual CatCurrencyDto CurrencyManagementFeeNavigation { get; set; }
        public virtual CatCurrencyDto CurrencyWireFeeNavigation { get; set; }
        public virtual CatCurrencyDto CurrencyPaymentAmountNavigation { get; set; }
        public virtual ConsultantContactsServiceDto SupplierNavigation { get; set; }
        public virtual SupplierPartnerProfileServiceDto SupplierPartnerNavigation { get; set; }
        public virtual ICollection<CommentPaymentConceptSelectDto> CommentPaymentConcepts { get; set; }
        public virtual ICollection<DocumentPaymentConceptDto> DocumentPaymentConcepts { get; set; }
        public virtual ICollection<RecurrencePaymentConceptDto> RecurrencePaymentConcepts { get; set; }
        public virtual ICollection<WireTransferPaymentConceptDto> WireTransferPaymentConcepts { get; set; }
        public virtual ICollection<WireTransferServicePaymentConceptSelectDto> WireTransferServicePaymentConcepts { get; set; }
        public virtual ICollection<ConceptDto> Concepts { get; set; }

    }
}
