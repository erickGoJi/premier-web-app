using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileConsultant
{
    public class AreasCoverageConsultantDto
    {
        public int Id { get; set; }
        public int? SupplierPartnerProfileConsultant { get; set; }
        public int? Type { get; set; }
        public int? Country { get; set; }
        public int? PrimaryCity { get; set; }
        public string CurrentAddress { get; set; }
        public string ZipCode { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatCountryDto CountryNavigation { get; set; }
        public virtual CatCityDto PrimaryCityNavigation { get; set; }
        public virtual SupplierPartnerProfileConsultantDto SupplierPartnerProfileConsultantNavigation { get; set; }
        public virtual CatAreaCoverageTypeDto TypeNavigation { get; set; }
        public virtual ICollection<AdministrativeContactsConsultantDto> AdministrativeContactsConsultants { get; set; }
        public virtual ICollection<CityAreasCoverageConsultantDto> CityAreasCoverageConsultants { get; set; }
        public virtual ICollection<ProfileUserDto> ProfileUsers { get; set; }
        public virtual ICollection<DocumentAreasCoverageConsultantDto> DocumentAreasCoverageConsultants { get; set; }
        public virtual ICollection<PaymentInformationConsultantDto> PaymentInformationConsultants { get; set; }
    }
}
