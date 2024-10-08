﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class CatResponsablePayment
    {
        public CatResponsablePayment()
        {
            AirportTransportationServices = new HashSet<AirportTransportationService>();
            HomeFindingInitialRentPayments = new HashSet<HomeFinding>();
            HomeFindingOngoingRentPayments = new HashSet<HomeFinding>();
            HomeFindingRealtorCommissions = new HashSet<HomeFinding>();
            HomeFindingSecurityDeposits = new HashSet<HomeFinding>();
            PaymentHousings = new HashSet<PaymentHousing>();
            PaymentRentals = new HashSet<PaymentRental>();
            PaymentSchoolings = new HashSet<PaymentSchooling>();
        }

        public int Id { get; set; }
        public string Responsable { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<AirportTransportationService> AirportTransportationServices { get; set; }
        public virtual ICollection<HomeFinding> HomeFindingInitialRentPayments { get; set; }
        public virtual ICollection<HomeFinding> HomeFindingOngoingRentPayments { get; set; }
        public virtual ICollection<HomeFinding> HomeFindingRealtorCommissions { get; set; }
        public virtual ICollection<HomeFinding> HomeFindingSecurityDeposits { get; set; }
        public virtual ICollection<PaymentHousing> PaymentHousings { get; set; }
        public virtual ICollection<PaymentRental> PaymentRentals { get; set; }
        public virtual ICollection<PaymentSchooling> PaymentSchoolings { get; set; }
    }
}