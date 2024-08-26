using System;

namespace api.premier.Models.ChatImmigrationRelocation
{
    public class ChatDocumentImmigrationRelocationDto
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public int ChatImmigrationRelocationId { get; set; }
        public DateTime DateComment { get; set; }
        public bool Status { get; set; }

        public virtual ChatImmigrationRelocationDto ChatImmigrationRelocation { get; set; }
    }
}