using System;

namespace biz.premier.Models.ClientPartnerProfile
{
    public class DocumentGeneralContractPricingInfoDto
    {
        public int Id { get; set; }
        public int IdGeneralContractPricingInfo { get; set; }
        public int IdDocumentType { get; set; }
        public DateTime UpdateDate { get; set; }
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public string FileRequest { get; set; }

        public virtual GeneralContractPricingInfoDto IdGeneralContractPricingInfoNavigation { get; set; }
    }
}