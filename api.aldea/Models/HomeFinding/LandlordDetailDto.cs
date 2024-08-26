using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class LandlordDetailDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public string Name { get; set; }
        public string PrincipalEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string PrincipalPhone { get; set; }
        public string SecondaryPhone { get; set; }

        public virtual HomeFindingDto HomeFinding { get; set; }
    }
}
