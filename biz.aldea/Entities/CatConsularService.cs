﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class CatConsularService
    {
        public CatConsularService()
        {
            CorporateAssistances = new HashSet<CorporateAssistance>();
            EntryVisas = new HashSet<EntryVisa>();
            ResidencyPermits = new HashSet<ResidencyPermit>();
            WorkPermits = new HashSet<WorkPermit>();
        }

        public int Id { get; set; }
        public string Service { get; set; }

        public virtual ICollection<CorporateAssistance> CorporateAssistances { get; set; }
        public virtual ICollection<EntryVisa> EntryVisas { get; set; }
        public virtual ICollection<ResidencyPermit> ResidencyPermits { get; set; }
        public virtual ICollection<WorkPermit> WorkPermits { get; set; }
    }
}