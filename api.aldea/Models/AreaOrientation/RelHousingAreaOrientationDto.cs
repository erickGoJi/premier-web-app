using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AreaOrientation
{
    public class RelHousingAreaOrientationDto
    {
        public int HousingSpecificationId { get; set; }
        public int AmenitieId { get; set; }
        public virtual HousingAreaOrientationDto HousingSpecification { get; set; }
    }
}
