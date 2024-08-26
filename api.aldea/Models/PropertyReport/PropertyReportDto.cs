using api.premier.Models.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PropertyReport
{
    public class PropertyReportDto
    {
        public int Id { get; set; }
        public int PropertyInspection { get; set; }
        public DateTime? ReportDate { get; set; }
        public int? HousingList { get; set; }
        public string PropertyAddress { get; set; }
        public int? ZipCode { get; set; }
        public string Notes { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? IdServiceDetail { get; set; }
        public int? IdStatus { get; set; }

        public virtual UserDto CreatedByNavigation { get; set; }
        public virtual HousingListDto HousingListNavigation { get; set; }
        public virtual ICollection<PropertyReportSectionDto> PropertyReportSections { get; set; }
        public virtual ICollection<KeyInventoryDto> KeyInventories { get; set; }
        public virtual ICollection<AttendeeDto> Attendees { get; set; }
    }
    public class PropertyReportSelectDto
    {
        public int Id { get; set; }
        public DateTime? ReportDate { get; set; }
        public int PropertyInspection { get; set; }
        public int? HousingList { get; set; }
        public string PropertyAddress { get; set; }
        public int? ZipCode { get; set; }
        public string Notes { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? IdStatus { get; set; }

        public virtual UserDto CreatedByNavigation { get; set; }
        public virtual HousingListDto HousingListNavigation { get; set; }
        public virtual ICollection<PropertyReportSectionDto> PropertyReportSections { get; set; }
        public virtual ICollection<KeyInventoryDto> KeyInventories { get; set; }
        public virtual ICollection<AttendeeDto> Attendees { get; set; }
    }
}
