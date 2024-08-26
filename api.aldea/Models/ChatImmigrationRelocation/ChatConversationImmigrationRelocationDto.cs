using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ChatImmigrationRelocation
{
    public class ChatConversationImmigrationRelocationDto
    {
        public ChatConversationImmigrationRelocationDto()
        {
            ChatImmigrationRelocations = new HashSet<ChatImmigrationRelocationDto>();
        }

        public int Id { get; set; }
        public int ServiceLineId { get; set; }
        public int? ServiceRecordId { get; set; }
        public bool Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<ChatImmigrationRelocationDto> ChatImmigrationRelocations { get; set; }

    }
}
