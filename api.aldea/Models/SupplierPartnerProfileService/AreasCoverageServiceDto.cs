using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileService
{
    public class AreasCoverageServiceDto
    {
        public int Id { get; set; }
        public int? SupplierPartnerProfileService { get; set; }
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
        public virtual SupplierPartnerProfileServiceDto SupplierPartnerProfileServiceNavigation { get; set; }
        public virtual CatAreaCoverageTypeDto TypeNavigation { get; set; }
        public virtual ICollection<AdministrativeContactsServiceDto> AdministrativeContactsServices { get; set; }
        public virtual ICollection<CityAreasCoverageServiceDto> CityAreasCoverageServices { get; set; }
        public virtual ICollection<ConsultantContactsServiceDto> ConsultantContactsServices { get; set; }
        public virtual ICollection<DocumentAreasCoverageServiceDto> DocumentAreasCoverageServices { get; set; }
        public virtual ICollection<PaymentInformationServiceDto> PaymentInformationServices { get; set; }
        public virtual ICollection<CampusDto> Campuses { get; set; }
    }
}
