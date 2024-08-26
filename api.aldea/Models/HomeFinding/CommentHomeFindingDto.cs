using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class CommentHomeFindingDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public string Reply { get; set; }
        public int? UserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual HomeFindingDto HomeFinding { get; set; }
        //public virtual UserDto User { get; set; }
    }
    public class CommentHomeFindingSelectDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public string Reply { get; set; }
        public int? UserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual HomeFindingDto HomeFinding { get; set; }
        public virtual UserDto User { get; set; }
    }
}
