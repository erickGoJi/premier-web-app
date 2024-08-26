using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using System.Collections.Generic;
using api.premier.Models.Countries;

namespace api.premier.Models.ClientPartnerProfile
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

        public virtual CitiesGenericDto IdCityNavigation { get; set; }
        //public virtual ClientPartnerProfileDto IdClientPartnerProfileNavigation { get; set; }
        public virtual CountriesGenericDto IdCountryNavigation { get; set; }
        public virtual StatesGenericDto IdStateNavigation { get; set; }
        public virtual ICollection<PaymentInformationOfficeDto> PaymentInformationOffices { get; set; }
        public virtual TypeOfficeDto IdTypeOfficeNavigation { get; set; }
        public virtual ICollection<DocumentOfficeInformationDto> DocumentOfficeInformations { get; set; }
        public virtual ICollection<OfficeContactDto> OfficeContacts { get; set; }
    }
}