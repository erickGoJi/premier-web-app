﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class CatPropertyTypeHousing
    {
        public CatPropertyTypeHousing()
        {
            HousingLists = new HashSet<HousingList>();
            PermanentHomes = new HashSet<PermanentHome>();
            TemporaryHousingCoordinatons = new HashSet<TemporaryHousingCoordinaton>();
        }

        public int Id { get; set; }
        public string PropertyType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<HousingList> HousingLists { get; set; }
        public virtual ICollection<PermanentHome> PermanentHomes { get; set; }
        public virtual ICollection<TemporaryHousingCoordinaton> TemporaryHousingCoordinatons { get; set; }
    }
}