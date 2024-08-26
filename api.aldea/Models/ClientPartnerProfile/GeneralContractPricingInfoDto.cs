using System;
using System.Collections.Generic;

namespace api.premier.Models.ClientPartnerProfile
{
    public class GeneralContractPricingInfoDto
    {
        public int Id { get; set; }
        public int IdClientPartnerProfile { get; set; }
        public DateTime ContractEffectiveDate { get; set; }
        public DateTime ContractExpirationDate { get; set; }
        public int? IdReferralFee { get; set; }
        public int IdPricingSchedule { get; set; }
        public int IdPaymentRecurrence { get; set; }
        public string Description { get; set; }
        public string ReferralFee { get; set; }

        public virtual ClientPartnerProfileDto IdClientPartnerProfileNavigation { get; set; }
        public virtual PaymentRecurrenceDto IdPaymentRecurrenceNavigation { get; set; }
        public virtual PricingScheduleDto IdPricingScheduleNavigation { get; set; }
        public virtual ICollection<DocumentGeneralContractPricingInfoDto> DocumentGeneralContractPricingInfos { get; set; }
    }
}