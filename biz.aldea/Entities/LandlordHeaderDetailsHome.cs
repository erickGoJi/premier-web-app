﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class LandlordHeaderDetailsHome
    {
        public LandlordHeaderDetailsHome()
        {
            LandlordDetailsHomes = new HashSet<LandlordDetailsHome>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PrincipalEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string PrincipalPhone { get; set; }
        public string SecundaryPhone { get; set; }
        public bool? FiscalInvoice { get; set; }
        public bool? Visible { get; set; }
        public int? IdServiceDetail { get; set; }
        public int? HousingListId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ServiceNumber { get; set; }
        public string ServiceTypeText { get; set; }
        public string WorkOrderText { get; set; }

        public virtual HousingList HousingList { get; set; }
        public virtual ICollection<LandlordDetailsHome> LandlordDetailsHomes { get; set; }
    }
}