using System;
using System.Collections.Generic;

namespace biz.premier.Models.ClientPartnerProfile
{
    public class PaymentInformationOfficeDto
    {
        public int Id { get; set; }
        public int? IdOfficeInformation { get; set; }
        public bool? FiscalInvoice { get; set; }
        public bool? CreditCard { get; set; }
        public bool? Checks { get; set; }
        public string PayToOrderOf { get; set; }
        public bool? Cash { get; set; }
        public string Comment { get; set; }
        public string GeneralComment { get; set; }

        public virtual ICollection<OfficeInformationDto> OfficeInformations { get; set; }
        public virtual ICollection<WireTransferPaymentInformationOfficeDto> WireTransferPaymentInformationOffices { get; set; }
    }
}