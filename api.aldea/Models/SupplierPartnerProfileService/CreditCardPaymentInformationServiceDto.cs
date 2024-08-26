using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileService
{
    public class CreditCardPaymentInformationServiceDto
    {
        public int PaymentInformationService { get; set; }
        public int CreditCard { get; set; }

        public virtual CatCreditCardDto CreditCardNavigation { get; set; }
        public virtual PaymentInformationServiceDto PaymentInformationServiceNavigation { get; set; }
    }
}
