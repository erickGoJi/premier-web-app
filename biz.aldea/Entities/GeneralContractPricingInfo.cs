﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class GeneralContractPricingInfo
    {
        public GeneralContractPricingInfo()
        {
            DocumentGeneralContractPricingInfos = new HashSet<DocumentGeneralContractPricingInfo>();
        }

        public int Id { get; set; }
        public int IdClientPartnerProfile { get; set; }
        public DateTime ContractEffectiveDate { get; set; }
        public DateTime ContractExpirationDate { get; set; }
        public int? IdReferralFee { get; set; }
        public int IdPricingSchedule { get; set; }
        public int IdPaymentRecurrence { get; set; }
        public string Description { get; set; }
        public string ReferralFee { get; set; }

        public virtual ClientPartnerProfile IdClientPartnerProfileNavigation { get; set; }
        public virtual CatPaymentRecurrence IdPaymentRecurrenceNavigation { get; set; }
        public virtual PricingSchedule IdPricingScheduleNavigation { get; set; }
        public virtual ReferralFee IdReferralFeeNavigation { get; set; }
        public virtual ICollection<DocumentGeneralContractPricingInfo> DocumentGeneralContractPricingInfos { get; set; }
    }
}