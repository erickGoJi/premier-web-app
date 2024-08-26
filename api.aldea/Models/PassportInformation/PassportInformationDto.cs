using api.premier.Models.ImmigrationProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PassportInformation
{
    public class PassportInformationDto
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime? Issue { get; set; }
        public DateTime? Expiration { get; set; }
        public string IssuingAuthority { get; set; }
        public string PlaceIssue { get; set; }
        public string CurrentAddress { get; set; }
        public string SpecificAttentionPoints { get; set; }
        public virtual ICollection<ImmigrationProfileInsertDto> ImmigrationProfiles { get; set; }

    }
}
