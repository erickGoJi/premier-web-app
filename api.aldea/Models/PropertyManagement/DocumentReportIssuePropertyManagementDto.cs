using System;

namespace api.premier.Models.PropertyManagement
{
    public class DocumentReportIssuePropertyManagementDto
    {
        public int Id { get; set; }
        public int? PropertyManagement { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public string NameFile { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual PropertyManagementDto PropertyManagementNavigation { get; set; }
    }
}