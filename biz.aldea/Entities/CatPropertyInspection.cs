﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class CatPropertyInspection
    {
        public CatPropertyInspection()
        {
            PropertyReports = new HashSet<PropertyReport>();
        }

        public int Id { get; set; }
        public string PropertyInspection { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<PropertyReport> PropertyReports { get; set; }
    }
}