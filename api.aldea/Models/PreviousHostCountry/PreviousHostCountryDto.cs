using api.premier.Models.ImmigrationProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PreviousHostCountry
{
    public class PreviousHostCountryDto
    {
        public bool? PriorHostCountryVisaIssued { get; set; }
        public int Id { get; set; }
        public string VisaNumber { get; set; }
        public DateTime? Issue { get; set; }
        public DateTime? Expiration { get; set; }
        public string IssuingAuthority { get; set; }
        public string PlaceIssue { get; set; }
        public int? VisaCategoryId { get; set; }
        public string IdAssignedNumber { get; set; }
        public string PositionEmployer { get; set; }
        public string PositionResponsabilities { get; set; }
        public virtual ICollection<ImmigrationProfileInsertDto> ImmigrationProfiles { get; set; }

    }
}
