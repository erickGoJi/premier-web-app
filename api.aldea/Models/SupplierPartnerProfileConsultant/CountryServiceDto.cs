using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.Models.Catalogos;

namespace api.premier.Models.SupplierPartnerProfileConsultant
{
    public class CountryServiceDto
    {
        public int Consultant { get; set; }
        public int Country { get; set; }
        public virtual CatCountryDto CountryNavigation { get; set; }
    }
}
