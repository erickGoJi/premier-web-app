using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Conversation
{
    public class UserGroupDto
    {
        public int Conversation { get; set; }
        public int UserReciver { get; set; }
        public virtual ConversationDto ConversationNavigation { get; set; }
        public virtual UserDto UserReciverNavigation { get; set; }
    }
}
