using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileConsultant
{
    public class LanguagesConsultantContactsConsultantDto
    {
        public int ConsultantContactsService { get; set; }
        public int Language { get; set; }

        public virtual ProfileUserDto ConsultantContactsServiceNavigation { get; set; }
        public virtual CatLanguagesDto LanguageNavigation { get; set; }
    }
}
