using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Task
{
    public class TaskReplyDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Reply { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
    public partial class TaskReplySelectDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Reply { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        public virtual UserDto CreatedByNavigation { get; set; }
    }
}
