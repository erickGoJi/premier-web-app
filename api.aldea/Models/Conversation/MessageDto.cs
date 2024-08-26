using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Conversation
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int? Conversation { get; set; }
        public int UserId { get; set; }
        public string Message1 { get; set; }
        public DateTime Time { get; set; }
        public bool Status { get; set; }

        public virtual ConversationDto ConversationNavigation { get; set; }
        public virtual UserDto User { get; set; }
        public virtual ICollection<DocumentMessageDto> DocumentMessages { get; set; }

    }
}
