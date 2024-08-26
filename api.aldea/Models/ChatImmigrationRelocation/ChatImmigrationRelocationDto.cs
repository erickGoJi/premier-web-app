using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace api.premier.Models.ChatImmigrationRelocation
{
    public class ChatImmigrationRelocationDto
    {
        public ChatImmigrationRelocationDto() 
        {
            ChatDocumentImmigrationRelocations = new HashSet<ChatDocumentImmigrationRelocationDto>();
        }

        public int Id { get; set; }
        public int ChatCoversationId { get; set; }
        public int UserId { get; set; }
        public int ChatDocumentId { get; set; }
        public string Comment { get; set; }
        public DateTime DateComment { get; set; }
        public bool Status { get; set; }

        public virtual ICollection <ChatDocumentImmigrationRelocationDto> ChatDocumentImmigrationRelocations { get; set; }
    }
}