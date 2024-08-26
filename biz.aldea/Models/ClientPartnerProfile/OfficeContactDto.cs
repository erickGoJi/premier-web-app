
namespace biz.premier.Models.ClientPartnerProfile
{
    public class OfficeContactDto
    {
        public int Id { get; set; }
        public int IdOfficeInformation { get; set; }
        public int IdContactType { get; set; }
        public string ContactName { get; set; }
        public string Tittle { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int IdCity { get; set; }
        public int IdCountry { get; set; }
        public int IdState { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }

        public virtual OfficeContactTypeDto IdContactTypeNavigation { get; set; }
        public virtual OfficeInformationDto IdOfficeInformationNavigation { get; set; }
    }
}