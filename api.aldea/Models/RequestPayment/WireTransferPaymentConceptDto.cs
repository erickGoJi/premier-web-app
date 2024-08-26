using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestPayment
{
    public class WireTransferPaymentConceptDto
    {
        public int Id { get; set; }
        public int PaymentConcept { get; set; }
        public int? AccountType { get; set; }
        public string AccountHoldersName { get; set; }
        public string BankName { get; set; }
        public long? AccountNumber { get; set; }
        public long? RoutingNumber { get; set; }
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

        public virtual CatBankAccountTypeDto AccountTypeNavigation { get; set; }
        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
    }
}
