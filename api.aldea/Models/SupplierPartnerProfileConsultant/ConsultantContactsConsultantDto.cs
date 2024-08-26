using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using api.premier.Models.ProfileUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileConsultant
{
    public class ProfileUserDto
    {
        public int Id { get; set; }
        public int? AreasCoverage { get; set; }
        public string Photo { get; set; }
        public string PhotoExtension { get; set; }
        public int? SupplierType { get; set; }
        public int? role_ID { get; set; }
        public string SupplierPartner { get; set; }
        public decimal? AmountPerHour { get; set; }
        public string Currency { get; set; }
        public string CreditTerms { get; set; }
        public string TaxesPercentage { get; set; }
        public string Vip { get; set; }
        public int? ResponsablePremierOffice { get; set; }
        public bool? Immigration { get; set; }
        public bool? Relocation { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string MotherLastName { get; set; }
        public int? Title { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? City { get; set; }
        public int? Country { get; set; }
        public string Comments { get; set; }
        public string ProfessionalBiography { get; set; }
        public int? UserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AreasCoverageConsultantDto AreasCoverageNavigation { get; set; }
        public virtual CatCityDto CityNavigation { get; set; }
        public virtual CatCountryDto CountryNavigation { get; set; }
        public virtual CatSupplierTypeDto SupplierTypeNavigation { get; set; }
        public virtual CatTitleDto TitleNavigation { get; set; }
        public virtual UserCreateDto User { get; set; }
        public virtual PersonalInformationDto PersonalInformation { get; set; }
        public virtual ICollection<AssignedTeamDto> AssignedTeamAssignedByNavigations { get; set; }
        public virtual ICollection<AssignedTeamDto> AssignedTeamAssignedToNavigations { get; set; }
        public virtual ICollection<OfficeDto> Offices { get; set; }
        public virtual ICollection<OperationLeaderDto> OperationLeaderConsultantNavigations { get; set; }
        public virtual ICollection<OperationLeaderDto> OperationLeaderCreatedByNavigations { get; set; }
        public virtual ICollection<CountryServiceDto> CountryServices { get; set; }
        public virtual ICollection<DocumentConsultantContactsConsultantDto> DocumentConsultantContactsConsultants { get; set; }
        public virtual ICollection<LanguagesConsultantContactsConsultantDto> LanguagesConsultantContactsConsultants { get; set; }
        public virtual ICollection<VehicleConsultantDto> VehicleConsultants { get; set; }
    }
}
