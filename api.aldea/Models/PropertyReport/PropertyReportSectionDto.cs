using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PropertyReport
{
    public class PropertyReportSectionDto
    {
        public int Id { get; set; }
        public int? PropertyReport { get; set; }
        public int? PropertySection { get; set; }
        public int? Status { get; set; }
        public bool? NeedRepair { get; set; }
        public DateTime? ReportDate { get; set; }
        public string ReportDetails { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool? NeedClean { get; set; }
        public bool? NeedReplace { get; set; }
        public bool? NoneAction { get; set; }

        public virtual PropertyReportDto PropertyReportNavigation { get; set; }
        //public virtual CatPropertySectionDto PropertySectionNavigation { get; set; }
        //public virtual CatStatusPropertySectionDto StatusNavigation { get; set; }
        public virtual ICollection<PhotosPropertyReportSectionDto> PhotosPropertyReportSections { get; set; }
        public virtual ICollection<SectionInventoryDto> SectionInventories { get; set; }
    }
}
