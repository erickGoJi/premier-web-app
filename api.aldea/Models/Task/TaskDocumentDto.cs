using System;

namespace api.premier.Models.Task
{
    public class TaskDocumentDto
    {
        public int Id { get; set; }
        public string FileRequest { get; set; }
        public string NameFile { get; set; }
        public string Comment { get; set; }
        public int TaskId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string FileExtension { get; set; }
        //public virtual TaskDto Task { get; set; }
    }
}