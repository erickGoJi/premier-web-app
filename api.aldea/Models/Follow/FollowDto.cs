using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Follow
{
    public class FollowDto
    {
        public int Id { get; set; }
        public int ServiceRecordId { get; set; }
        public int UserId { get; set; }
    }
}
