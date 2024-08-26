﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class WireTransferService
    {
        public WireTransferService()
        {
            WireTransferServicePaymentConcepts = new HashSet<WireTransferServicePaymentConcept>();
        }

        public int Id { get; set; }
        public int? PaymentInformationService { get; set; }
        public int? AccountType { get; set; }
        public string AccountHoldersName { get; set; }
        public string BankName { get; set; }
        public int? AccountNumber { get; set; }
        public int? RoutingNumber { get; set; }
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

        public virtual CatBankAccountType AccountTypeNavigation { get; set; }
        public virtual CatCurrency CurrencyNavigation { get; set; }
        public virtual PaymentInformationService PaymentInformationServiceNavigation { get; set; }
        public virtual ICollection<WireTransferServicePaymentConcept> WireTransferServicePaymentConcepts { get; set; }
    }
}