﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class TypeVehiclesSupplierPartnerDetail
    {
        public int SupplairPartnerDetail { get; set; }
        public int TypeVehicles { get; set; }

        public virtual SupplierPartnerDetail SupplairPartnerDetailNavigation { get; set; }
        public virtual CatVehicleType TypeVehiclesNavigation { get; set; }
    }
}