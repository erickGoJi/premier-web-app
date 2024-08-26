using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.Models.Catalogos;

namespace api.premier.Models.SupplierPartnerProfileConsultant
{
    public class OfficeDto
    {
        public int Consultant { get; set; }
        public int Office1 { get; set; }
        public virtual ProfileUserDto ConsultantNavigation { get; set; }
        public virtual CatOfficeDto Office1Navigation { get; set; }
    }
}
