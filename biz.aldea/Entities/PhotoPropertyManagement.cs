﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class PhotoPropertyManagement
    {
        public int Id { get; set; }
        public int? PropertyManagement { get; set; }
        public string Photo { get; set; }
        public string NamePhoto { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual PropertyManagement PropertyManagementNavigation { get; set; }
    }
}