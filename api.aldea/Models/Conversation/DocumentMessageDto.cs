using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Conversation
{
    public class DocumentMessageDto
    {
        public int Id { get; set; }
        public int? Message { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public DateTime? Date { get; set; }
        public bool? Status { get; set; }

        public virtual MessageDto MessageNavigation { get; set; }
    }
}
