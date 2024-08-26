using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class LandlordDetailsHomeDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string PrincipalEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string PrincipalPhone { get; set; }
        public string SecundaryPhone { get; set; }
        public bool? FiscalInvoice { get; set; }
        public bool? CreditCard { get; set; }
        public bool? WireTransfer { get; set; }
        public int? AccountType { get; set; }
        public string AccountHoldersName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public string SwiftBicCode { get; set; }
        public int? Currency { get; set; }
        public decimal? WireFree { get; set; }
        public string BankAddress { get; set; }
        public bool? InternationalPayment { get; set; }
        public bool? Checks { get; set; }
        public string PayToOrderOf { get; set; }
        public bool? Cash { get; set; }
        public string Comment { get; set; }
        public string GeneralComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int IdLandlord { get; set; }
        public int? IdServiceDetail { get; set; }

        public bool? Visible { get; set; }

        public string Clabetype { get; set; }

        public int? HeaderId { get; set; }

        public virtual CatBankAccountTypeDto AccountTypeNavigation { get; set; }
        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        //public virtual HousingListDto IdNavigation { get; set; }
       // public virtual ICollection<CreditCardLandLordDetailDto> CreditCardLandLordDetails { get; set; }
    }
}
