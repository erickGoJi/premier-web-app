using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileService
{
    public class CityAreasCoverageServiceDto
    {
        public int AreasCoverageService { get; set; }
        public int City { get; set; }

        public virtual AreasCoverageServiceDto AreasCoverageServiceNavigation { get; set; }
        public virtual CatCityDto CityNavigation { get; set; }
    }
}
