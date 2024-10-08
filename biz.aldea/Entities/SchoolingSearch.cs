﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class SchoolingSearch
    {
        public SchoolingSearch()
        {
            CommentSchoolingSearches = new HashSet<CommentSchoolingSearch>();
            DocumentSchoolingSearches = new HashSet<DocumentSchoolingSearch>();
            ExtensionSchoolingSearches = new HashSet<ExtensionSchoolingSearch>();
            KeyReloDetailServices = new HashSet<KeyReloDetailService>();
            PaymentSchoolings = new HashSet<PaymentSchooling>();
            ReminderSchoolingSearches = new HashSet<ReminderSchoolingSearch>();
            SchoolingInformations = new HashSet<SchoolingInformation>();
        }

        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
        public int StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public string Comment { get; set; }
        public int? OneTimeEntryFee { get; set; }
        public int? AdmissionsFee { get; set; }
        public int? Tution { get; set; }
        public int? Books { get; set; }
        public int? Uniform { get; set; }
        public int? Ib { get; set; }
        public string Others { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatStatusWorkOrder Status { get; set; }
        public virtual WorkOrderService WorkOrderServices { get; set; }
        public virtual ICollection<CommentSchoolingSearch> CommentSchoolingSearches { get; set; }
        public virtual ICollection<DocumentSchoolingSearch> DocumentSchoolingSearches { get; set; }
        public virtual ICollection<ExtensionSchoolingSearch> ExtensionSchoolingSearches { get; set; }
        public virtual ICollection<KeyReloDetailService> KeyReloDetailServices { get; set; }
        public virtual ICollection<PaymentSchooling> PaymentSchoolings { get; set; }
        public virtual ICollection<ReminderSchoolingSearch> ReminderSchoolingSearches { get; set; }
        public virtual ICollection<SchoolingInformation> SchoolingInformations { get; set; }
    }
}