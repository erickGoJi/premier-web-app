﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class Country1
    {
        public Country1()
        {
            CountryGalleries = new HashSet<CountryGallery>();
        }

        public int Id { get; set; }
        public string Country { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int? UpdatedDate { get; set; }

        public virtual ICollection<CountryGallery> CountryGalleries { get; set; }
    }
}