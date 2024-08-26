using System;

namespace api.premier.Models.PropertyManagement
{
    public class PhotoInspectionPropertyManagementDto
    {
        public int Id { get; set; }
        public int? PropertyManagement { get; set; }
        public string Photo { get; set; }
        public string PhotoExtension { get; set; }
        public string NamePhoto { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual PropertyManagementDto PropertyManagementNavigation { get; set; }
    }
}