using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileService
{
    public class PaymentInformationServiceDto
    {
        public int Id { get; set; }
        public int? AreasCoverageService { get; set; }
        public bool? FiscalInvoice { get; set; }
        public bool? CreditCard { get; set; }
        public bool? Checks { get; set; }
        public string PayToOrderOf { get; set; }
        public bool? Cash { get; set; }
        public string Comment { get; set; }
        public string GeneralComment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AreasCoverageServiceDto AreasCoverageServiceNavigation { get; set; }
        public virtual ICollection<CreditCardPaymentInformationServiceDto> CreditCardPaymentInformationServices { get; set; }
        public virtual ICollection<WireTransferServiceDto> WireTransferServices { get; set; }
    }
}
