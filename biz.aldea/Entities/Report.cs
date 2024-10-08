﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class Report
    {
        public Report()
        {
            Columns = new HashSet<Column>();
            Filters = new HashSet<Filter>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ReportType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatReport ReportTypeNavigation { get; set; }
        public virtual ICollection<Column> Columns { get; set; }
        public virtual ICollection<Filter> Filters { get; set; }
    }
}