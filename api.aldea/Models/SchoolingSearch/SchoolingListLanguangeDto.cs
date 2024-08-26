using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SchoolingSearch
{
    public class SchoolingListLanguangeDto
    {
        public int Id { get; set; }
        public int? IdLanguage { get; set; }
        public int? IdSchoolingList { get; set; }

        public virtual CatLanguage IdLanguageNavigation { get; set; }
        public virtual SchoolsList IdSchoolingListNavigation { get; set; }
    }
}
