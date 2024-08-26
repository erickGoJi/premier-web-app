using System;

namespace api.premier.Models.Training
{
    public class FileDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string Path { get; set; }
        public int Theme { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ThemeDto ThemeNavigation { get; set; }
    }
}