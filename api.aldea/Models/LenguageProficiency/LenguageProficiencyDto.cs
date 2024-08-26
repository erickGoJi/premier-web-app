using api.premier.Models.ImmigrationProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.LenguageProficiency
{
    public class LenguageProficiencyDto
    {
        public int Id { get; set; }
        public int? LanguageId { get; set; }
        public int? ProficiencyId { get; set; }
        public string Comments { get; set; }
        public int? ImmigrationProfileId { get; set; }
        public virtual ImmigrationProfileInsertDto ImmigrationProfile { get; set; }

    }
}
