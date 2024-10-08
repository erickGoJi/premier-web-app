﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class ReportDay
    {
        public ReportDay()
        {
            Appointments = new HashSet<Appointment>();
            ServiceReportDays = new HashSet<ServiceReportDay>();
        }

        public int Id { get; set; }
        public int? ReportNo { get; set; }
        public int? ReportBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ReportDate { get; set; }
        public int? ServiceLine { get; set; }
        public int? WorkOrder { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string TotalTime { get; set; }
        public string Activity { get; set; }
        public string Conclusion { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual User ReportByNavigation { get; set; }
        public virtual CatServiceLine ServiceLineNavigation { get; set; }
        public virtual WorkOrder WorkOrderNavigation { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<ServiceReportDay> ServiceReportDays { get; set; }
    }
}