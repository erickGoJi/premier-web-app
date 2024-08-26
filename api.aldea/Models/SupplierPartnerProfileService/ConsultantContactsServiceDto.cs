using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileService
{
    public class ConsultantContactsServiceDto
    {
        public int Id { get; set; }
        public int? AreasCoverage { get; set; }
        public string Photo { get; set; }
        public string PhotoExtension { get; set; }
        public int? SupplierType { get; set; }
        public bool? Immigration { get; set; }
        public bool? Relocation { get; set; }
        public string ContactName { get; set; }
        public string Title { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? City { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AreasCoverageServiceDto AreasCoverageNavigation { get; set; }
        public virtual CatCityDto CityNavigation { get; set; }
        public virtual CatSupplierTypeDto SupplierTypeNavigation { get; set; }
        public virtual ICollection<DocumentConsultantContactsServiceDto> DocumentConsultantContactsServices { get; set; }
        public virtual ICollection<LanguagesConsultantContactsServiceDto> LanguagesConsultantContactsServices { get; set; }
        public virtual ICollection<VehicleServiceDto> VehicleServices { get; set; }
    }
}
