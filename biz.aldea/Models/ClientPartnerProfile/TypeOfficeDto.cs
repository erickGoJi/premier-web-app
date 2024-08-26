using System.Collections.Generic;

namespace biz.premier.Models.ClientPartnerProfile
{
    public class TypeOfficeDto
    {
        public int Id { get; set; }
        public string Type { get; set; }

        public virtual ICollection<OfficeInformationDto> OfficeInformations { get; set; }
    }
}