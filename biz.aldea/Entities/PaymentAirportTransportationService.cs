﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class PaymentAirportTransportationService
    {
        public int Id { get; set; }
        public int? AirportTransportationServicesId { get; set; }
        public string PaymentType { get; set; }
        public string PaymentResponsibility { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AirportTransportationService AirportTransportationServices { get; set; }
    }
}