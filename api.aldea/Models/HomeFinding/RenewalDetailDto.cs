using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class RenewalDetailDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public bool? AutomaticallyRenewal { get; set; }
        public bool? RenewalNotification { get; set; }
        public DateTime? AdditionalRent { get; set; }

        public virtual HomeFindingDto HomeFinding { get; set; }
    }
}
