using System;

namespace api.premier.Models.TenancyManagement
{
    public class AssignedPhotoDto
    {
        public int Id { get; set; }
        public int ReportAnEventId { get; set; }
        public string PhotoName { get; set; }
        public string PhotoPath { get; set; }
        public string PhotoExtension { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}