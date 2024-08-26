using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ClientPartnerProfile
{
    public class ClientPartnerProfileDto
    {

        public int Id { get; set; }
        public int? IdTypePartnerClientProfile { get; set; }
        public int? BelongsToPartner { get; set; }
        public string Photo { get; set; }
        public string PhotoExtension { get; set; }
        public string Name { get; set; }
        public int? IdCompanyType { get; set; }
        public int? Industry { get; set; }
        public DateTime? PartnerClientSince { get; set; }
        public DateTime? ApprovalFrom { get; set; }
        public int? IdResponsiblePremierOffice { get; set; }
        public int? AssignedTo { get; set; }
        public int? IdLifeCircle { get; set; }
        public int? IdSuccessProbability { get; set; }
        public string IdReferralFee { get; set; }
        public int? IdPaymentRecurrence { get; set; }
        public int? IdPricingType { get; set; }
        public string About { get; set; }
        public int? IdStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CompanyTypeDto IdCompanyTypeNavigation { get; set; }
        public virtual LifeCircleDto IdLifeCircleNavigation { get; set; }
        public virtual PaymentRecurrenceDto IdPaymentRecurrenceNavigation { get; set; }
        public virtual PricingTypeDto IdPricingTypeNavigation { get; set; }
        public virtual ResponsiblePremierOfficeDto IdResponsiblePremierOfficeNavigation { get; set; }
        public virtual SuccessProbabilityDto IdSuccessProbabilityNavigation { get; set; }
        public virtual TypePartnerClientProfileDto IdTypePartnerClientProfileNavigation { get; set; }
        public virtual ICollection<ActivityLogDto> ActivityLogs { get; set; }
        public virtual ICollection<ClientPartnerProfileClientDto> ClientPartnerProfileClientIdClientFromNavigations { get; set; }
        public virtual ICollection<ClientPartnerProfileClientDto> ClientPartnerProfileClientIdClientToNavigations { get; set; }
        public virtual ICollection<ClientPartnerProfileExperienceTeamDto> ClientPartnerProfileExperienceTeams { get; set; }
        public virtual ICollection<DocumentClientPartnerProfileDto> DocumentClientPartnerProfiles { get; set; }
        public virtual ICollection<GeneralContractPricingInfoDto> GeneralContractPricingInfos { get; set; }
        public virtual ICollection<OfficeInformationDto> OfficeInformations { get; set; }
        public virtual ICollection<ServiceLocationDto> ServiceLocations { get; set; }
        public virtual ICollection<ServiceScoreAwardDto> ServiceScoreAwards { get; set; }
        public virtual ICollection<TermsDealDto> TermsDeals { get; set; }
    }
}
