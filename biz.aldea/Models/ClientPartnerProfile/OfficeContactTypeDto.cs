using System.Collections.Generic;

namespace biz.premier.Models.ClientPartnerProfile
{
    public class OfficeContactTypeDto
    {
        public int Id { get; set; }
        public string ContactType { get; set; }

        public virtual ICollection<OfficeContactDto> OfficeContacts { get; set; }
    }
}