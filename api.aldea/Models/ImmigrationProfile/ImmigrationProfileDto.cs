using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ImmigrationProfile
{
    public class ImmigrationProfileDto
    {
        public int Id { get; set; }
        public int? ServiceRecordId { get; set; }
        public int? PassportInformationId { get; set; }
        public int? PreviousHostCountryId { get; set; }
        public int? AssigmentInformationId { get; set; }
    }
}
