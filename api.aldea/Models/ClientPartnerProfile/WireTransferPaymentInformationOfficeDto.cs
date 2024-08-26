using System;

namespace api.premier.Models.ClientPartnerProfile
{
    public class WireTransferPaymentInformationOfficeDto
    {
        public int Id { get; set; }
        public int IdPaymentInformationOffice { get; set; }
        public int? AccountType { get; set; }
        public string AccountHoldersName { get; set; }
        public string BankName { get; set; }
        public int? AccountNumber { get; set; }
        public int? RoutingNumber { get; set; }
        public string SwiftBicCode { get; set; }
        public int? Currency { get; set; }
        public int? WireFeeApprox { get; set; }
        public string BankAddress { get; set; }
        public bool? InternationalPaymentAcceptance { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual PaymentInformationOfficeDto IdPaymentInformationOfficeNavigation { get; set; }
    }
}