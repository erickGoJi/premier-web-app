using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ProfileUser
{
    public class PersonalInformationDto
    {
        public int Id { get; set; }
        public int? Country { get; set; }
        public int? City { get; set; }
        public string HobbiesPassions { get; set; }
        public string CurrentAddress { get; set; }
        public int? ZipCode { get; set; }
        public DateTime? DateBirth { get; set; }
        public string Nationality { get; set; }
        public string PersonalEmail { get; set; }
        public string PersonalPhone { get; set; }
        public string Allergies { get; set; }
        public string School { get; set; }
        public string Major { get; set; }
        public DateTime? Graduation { get; set; }
        public decimal? TotalCompesation { get; set; }
        public string RhComments { get; set; }
        public string GeneralComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<CompesationBenefitDto> CompesationBenefits { get; set; }
        public virtual ICollection<DocumentProfileDto> DocumentProfiles { get; set; }
        public virtual ICollection<EmergencyContactDto> EmergencyContacts { get; set; }
        public virtual ICollection<PaymentInformationProfileDto> PaymentInformationProfiles { get; set; }
    }
    public class PersonalInformationSelectDto
    {
        public int Id { get; set; }
        public int? Country { get; set; }
        public int? City { get; set; }
        public string HobbiesPassions { get; set; }
        public string CurrentAddress { get; set; }
        public int? ZipCode { get; set; }
        public DateTime? DateBirth { get; set; }
        public string Nationality { get; set; }
        public string PersonalEmail { get; set; }
        public string PersonalPhone { get; set; }
        public string Allergies { get; set; }
        public string School { get; set; }
        public string Major { get; set; }
        public DateTime? Graduation { get; set; }
        public decimal? TotalCompesation { get; set; }
        public string RhComments { get; set; }
        public string GeneralComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatCityDto CityNavigation { get; set; }
        public virtual CatCountryDto CountryNavigation { get; set; }
        public virtual ICollection<CompesationBenefitSelectDto> CompesationBenefits { get; set; }
        public virtual ICollection<EmergencyContactSelectDto> EmergencyContacts { get; set; }
        public virtual ICollection<PaymentInformationProfileDto> PaymentInformationProfiles { get; set; }
    }
}
