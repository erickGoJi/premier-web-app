using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ProfileUser
{
    public class PaymentInformationProfileDto
    {
        public int Id { get; set; }
        public int? Profile { get; set; }
        public bool? WireTransfer { get; set; }
        public bool? FiscalInvoice { get; set; }
        public int? AccountType { get; set; }
        public string AccountHoldersName { get; set; }
        public string BankName { get; set; }
        public int? AccountNumber { get; set; }
        public int? RoutingNumber { get; set; }
        public string SwiftBicCode { get; set; }
        public int? Currency { get; set; }
        public int? Clabe { get; set; }
        public int? WireFeeApprox { get; set; }
        public string BankAddress { get; set; }
        public bool? InternationalPaymentAcceptance { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
