﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class HomeFindingRepair
    {
        public HomeFindingRepair()
        {
            DocumentRepairHomeFindings = new HashSet<DocumentRepairHomeFinding>();
        }

        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public int? RepairType { get; set; }
        public int? SupplierPartner { get; set; }
        public DateTime? RepairStartDate { get; set; }
        public DateTime? RepairEndDate { get; set; }
        public int? TotalDays { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatRepairType RepairTypeNavigation { get; set; }
        public virtual CatSupplierCompany SupplierPartnerNavigation { get; set; }
        public virtual ICollection<DocumentRepairHomeFinding> DocumentRepairHomeFindings { get; set; }
    }
}