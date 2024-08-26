using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PredicisionOrientation
{
    public class RelHousingDto
    {
        public int HousingSpecificationId { get; set; }
        public int AmenitieId { get; set; }
        public virtual HousingPredecisionOrientationDto HousingSpecification { get; set; }
    }
}
