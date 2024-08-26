using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.DependentInformationLanguage
{
    public class DependentInformationLanguageDto
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public int DependentInformationId { get; set; }

        public virtual CatLanguagesDto Language { get; set; }
    }
}
