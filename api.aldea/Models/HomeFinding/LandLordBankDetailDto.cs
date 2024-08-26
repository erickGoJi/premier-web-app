using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class LandLordBankDetailDto
    {
        public int Id { get; set; }
        public int? LandlordBankId { get; set; }
        public int? PaymentMethod { get; set; }
        public int? CreditCard { get; set; }
        public bool? FiscalInvoice { get; set; }
        public int? AccountType { get; set; }
        public string AccountHoldersName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public string SwiftBicCode { get; set; }
        public int? Currency { get; set; }
        public decimal? WireFeeApprox { get; set; }
        public string BankAddress { get; set; }
        public bool? InternationalPaymentAcceptance { get; set; }
        public string PayToOrderOf { get; set; }
        public string Comment { get; set; }

        //public virtual CatBankAccountTypeDto AccountTypeNavigation { get; set; }
        //public virtual CatCreditCardDto CreditCardNavigation { get; set; }
        public virtual LandLordBankDto LandlordBank { get; set; }
        //public virtual CatPaymetMethodDto PaymentMethodNavigation { get; set; }
    }
}
