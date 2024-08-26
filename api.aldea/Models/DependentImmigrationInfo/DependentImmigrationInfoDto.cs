using api.premier.Models.DependentInformations;
using api.premier.Models.ImmigrationProfile;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.DependentImmigrationInfo
{
    public class DependentImmigrationInfoDto
    {
        public int Id { get; set; }
        public int? RelationshipId { get; set; }
        public string Name { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? Issue { get; set; }
        public DateTime? Expiration { get; set; }
        public string IssuingAuthority { get; set; }
        public string PlaceIssue { get; set; }
        public DateTime? EntryDateHostCountry { get; set; }
        public string SpecificAttentionPoints { get; set; }
        public int? ImmigrationProfileId { get; set; }
        public int? DependentInformation { get; set; }
        public virtual ImmigrationProfileInsertDto ImmigrationProfile { get; set; }
        public virtual ICollection<DocumentDependentInformationDto> DocumentDependentImmigrationInfos { get; set; }

    }
}
