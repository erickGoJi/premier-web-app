using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class RelHousingHomeFindingDto
    {
        public int HousingHomeFindingId { get; set; }
        public int AmenitieId { get; set; }

        public virtual HousingHomeFindingDto HousingHomeFinding { get; set; }
    }
}
