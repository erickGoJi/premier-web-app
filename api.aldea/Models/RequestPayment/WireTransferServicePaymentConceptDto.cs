using api.premier.Models.SupplierPartnerProfileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestPayment
{
    public class WireTransferServicePaymentConceptDto
    {
        public int PaymentConcept { get; set; }
        public int WireTransferService { get; set; }
    }
    public class WireTransferServicePaymentConceptSelectDto
    {
        public int PaymentConcept { get; set; }
        public int WireTransferService { get; set; }
        public virtual PaymentSelectDto PaymentConceptNavigation { get; set; }
        public virtual WireTransferServiceDto WireTransferServiceNavigation { get; set; }
    }
}
