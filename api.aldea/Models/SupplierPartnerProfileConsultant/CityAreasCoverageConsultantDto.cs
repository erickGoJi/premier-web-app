using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileConsultant
{
    public class CityAreasCoverageConsultantDto
    {
        public int AreasCoverageConsultant { get; set; }
        public int City { get; set; }

        public virtual AreasCoverageConsultantDto AreasCoverageConsultantNavigation { get; set; }
        public virtual CatCityDto CityNavigation { get; set; }
    }
}
