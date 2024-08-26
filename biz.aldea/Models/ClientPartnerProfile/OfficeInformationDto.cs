using System.Collections.Generic;

namespace biz.premier.Models.ClientPartnerProfile
{
    public class OfficeInformationDto
    {
        public int Id { get; set; }
        public int IdClientPartnerProfile { get; set; }
        public int IdTypeOffice { get; set; }
        public string CommercialName { get; set; }
        public string LegalName { get; set; }
        public int IdCountry { get; set; }
        public int IdCity { get; set; }
        public int IdState { get; set; }
        public string CurrentAddress { get; set; }
        public int ZipCode { get; set; }
        public string TypeOffice { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public virtual ICollection<PaymentInformationOfficeDto> PaymentInformationOffices { get; set; }
        public virtual TypeOfficeDto IdTypeOfficeNavigation { get; set; }
        public virtual ICollection<DocumentOfficeInformationDto> DocumentOfficeInformations { get; set; }
        public virtual ICollection<OfficeContactDto> OfficeContacts { get; set; }
    }
}