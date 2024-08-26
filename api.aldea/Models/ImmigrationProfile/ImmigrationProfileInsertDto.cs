using api.premier.Models.AssigmentInformation;
using api.premier.Models.DependentImmigrationInfo;
using api.premier.Models.EducationalBackground;
using api.premier.Models.LenguageProficiency;
using api.premier.Models.PassportInformation;
using api.premier.Models.PreviousHostCountry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ImmigrationProfile
{
    public class ImmigrationProfileInsertDto
    {
        //public ImmigrationProfileInsertDto()
        //{
        //    DependentImmigrationInfos = new HashSet<DependentImmigrationInfoDto>();
        //    EducationalBackgrounds = new HashSet<EducationalBackgroundDto>();
        //    LenguageProficiencies = new HashSet<LenguageProficiencyDto>();
        //}
        public int Id { get; set; }
        public int? ServiceRecordId { get; set; }
        public int? PassportInformationId { get; set; }
        public int? PreviousHostCountryId { get; set; }
        public int? AssigmentInformationId { get; set; }
        public int? HighestLevelEducationalId { get; set; }
        public virtual AssigmentInformationDto AssigmentInformation { get; set; }
        public virtual PassportInformationDto PassportInformation { get; set; }
        public virtual PreviousHostCountryDto PreviousHostCountry { get; set; }
        public virtual ICollection<DependentImmigrationInfoDto> DependentImmigrationInfos { get; set; }
        public virtual ICollection<EducationalBackgroundDto> EducationalBackgrounds { get; set; }
        public virtual ICollection<LenguageProficiencyDto> LenguageProficiencies { get; set; }
    }
}
