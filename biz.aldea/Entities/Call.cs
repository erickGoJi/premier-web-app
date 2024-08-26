﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class Call
    {
        public Call()
        {
            CallAssistants = new HashSet<CallAssistant>();
        }

        public int Id { get; set; }
        public int ServiceRecordId { get; set; }
        public int Caller { get; set; }
        public int Calle { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int Duration { get; set; }
        public bool WelcomeCall { get; set; }
        public int WorkOrderId { get; set; }
        public int ServiceId { get; set; }
        public int? ServiceLineId { get; set; }
        public bool? Escalate { get; set; }
        public string Comments { get; set; }

        public virtual User CalleNavigation { get; set; }
        public virtual User CallerNavigation { get; set; }
        public virtual CatDuration DurationNavigation { get; set; }
        public virtual WorkOrderService Service { get; set; }
        public virtual CatServiceLine ServiceLine { get; set; }
        public virtual ServiceRecord ServiceRecord { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public virtual ICollection<CallAssistant> CallAssistants { get; set; }
    }
}