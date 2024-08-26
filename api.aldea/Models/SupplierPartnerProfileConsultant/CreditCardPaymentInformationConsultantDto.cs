using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileConsultant
{
    public class CreditCardPaymentInformationConsultantDto
    {
        public int PaymentInformationConsultant { get; set; }
        public int CreditCard { get; set; }

        public virtual CatCreditCardDto CreditCardNavigation { get; set; }
        public virtual PaymentInformationConsultantDto PaymentInformationConsultantNavigation { get; set; }
    }
}
