﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class PhotosInspec
    {
        public int PropertyReportSectionId { get; set; }
        public string PhotoName { get; set; }
        public string Photo { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int Id { get; set; }
        public int? Inspection { get; set; }

        public virtual Inspection InspectionNavigation { get; set; }
    }
}