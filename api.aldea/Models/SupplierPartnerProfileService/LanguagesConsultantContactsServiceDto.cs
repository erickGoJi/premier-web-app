using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileService
{
    public class LanguagesConsultantContactsServiceDto
    {
        public int ConsultantContactsService { get; set; }
        public int Language { get; set; }

        public virtual ConsultantContactsServiceDto ConsultantContactsServiceNavigation { get; set; }
        public virtual CatLanguagesDto LanguageNavigation { get; set; }
    }
}
