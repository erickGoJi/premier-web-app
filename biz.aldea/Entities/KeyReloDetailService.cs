﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class KeyReloDetailService
    {
        public int Id { get; set; }
        public int? KeyTableServicesId { get; set; }
        public int? HomeFindingId { get; set; }
        public int? HomePurchaseId { get; set; }
        public int? HomeSaleId { get; set; }
        public int? SettlingInId { get; set; }
        public int? RentalFurnitureCoordinationId { get; set; }
        public int? OtherId { get; set; }
        public int? SchoolingSearchId { get; set; }
        public int? PredecisionOrientationId { get; set; }
        public int? AreaOrientationId { get; set; }
        public int? DepartureId { get; set; }
        public int? TemporaryHousingCoordinatonId { get; set; }
        public int? TransportationId { get; set; }
        public int? AirportTransportationServicesId { get; set; }
        public int? LeaseRenewalId { get; set; }
        public int? PropertyManagementId { get; set; }
        public int? TenancyManagementId { get; set; }

        public virtual AirportTransportationService AirportTransportationServices { get; set; }
        public virtual AreaOrientation AreaOrientation { get; set; }
        public virtual Departure Departure { get; set; }
        public virtual HomeFinding HomeFinding { get; set; }
        public virtual HomePurchase HomePurchase { get; set; }
        public virtual HomeSale HomeSale { get; set; }
        public virtual KeyTableService KeyTableServices { get; set; }
        public virtual LeaseRenewal LeaseRenewal { get; set; }
        public virtual Other Other { get; set; }
        public virtual PredecisionOrientation PredecisionOrientation { get; set; }
        public virtual PropertyManagement PropertyManagement { get; set; }
        public virtual RentalFurnitureCoordination RentalFurnitureCoordination { get; set; }
        public virtual SchoolingSearch SchoolingSearch { get; set; }
        public virtual SettlingIn SettlingIn { get; set; }
        public virtual TemporaryHousingCoordinaton TemporaryHousingCoordinaton { get; set; }
        public virtual TenancyManagement TenancyManagement { get; set; }
        public virtual Transportation Transportation { get; set; }
    }
}