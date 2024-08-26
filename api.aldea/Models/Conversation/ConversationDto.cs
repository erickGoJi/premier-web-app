using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Conversation
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public int? UserTo { get; set; }
        public int? UserReciver { get; set; }

        public int? serviceRecordId { get; set; }
        public bool? Status { get; set; }
        public bool? Groups { get; set; }
        public string GroupName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual UserDto UserReciverNavigation { get; set; }
        public virtual UserDto UserToNavigation { get; set; }
        public virtual ICollection<MessageDto> Messages { get; set; }
        public virtual ICollection<UserGroupDto> UserGroups { get; set; }
    }
}
