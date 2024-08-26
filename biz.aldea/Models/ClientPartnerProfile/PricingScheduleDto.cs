using System.Collections.Generic;

namespace biz.premier.Models.ClientPartnerProfile
{
    public class PricingScheduleDto
    {
        public int Id { get; set; }
        public string PricingSchedule1 { get; set; }

        public virtual ICollection<GeneralContractPricingInfoDto> GeneralContractPricingInfos { get; set; }
    }
}