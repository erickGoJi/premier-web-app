using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using api.premier.Models.DependentInformations;
using api.premier.Models.SchoolingSearch;
using api.premier.Models.SupplierPartnerProfileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class SchoolsListDto
    {
        public int Id { get; set; }
        public int? WorkOrder { get; set; }
        public int? Service { get; set; }
        public int? ServiceType { get; set; }
        public int? SchoolNo { get; set; }
        public DateTime? VisitDate { get; set; }
        public DateTime? VisitDateTime { get; set; }
        public int? SchoolingStatus { get; set; }
        public int? Dependent { get; set; }
        public string SchoolName { get; set; }
        public int? Country { get; set; }
        public int? City { get; set; }
        public string Address { get; set; }
        public int? Grade { get; set; }
        public int? Languages { get; set; }
        public string WebSite { get; set; }
        public bool? Ib { get; set; }
        public bool? Uniform { get; set; }
        public bool? MedicalRecordNeeded { get; set; }
        public string Admision { get; set; }
        public decimal? PerMonth { get; set; }
        public decimal? ForEnrollment { get; set; }
        public decimal? FeesEnrollment { get; set; }
        public int? Currency { get; set; }
        public string AdditionalComments { get; set; }
        public int? SchoolingSearchId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? SupplierId { get; set; }
        public bool? SendSchool { get; set; }

        public virtual CatCityDto CityNavigation { get; set; }
        public virtual CatCountryDto CountryNavigation { get; set; }
        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        public virtual DependentInformationDto DependentNavigation { get; set; }
        public virtual CatGradeSchoolingDto GradeNavigation { get; set; }
        public virtual CatLanguagesDto LanguagesNavigation { get; set; }
        //public virtual WorkOrderDto WorkOrderNavigation { get; set; }
        public virtual CatSchoolStatusDto SchoolingStatusNavigation { get; set; }
        public virtual SupplierPartnerProfileServiceDto SupplierPartnerProfileServiceDto { get; set; }

        public virtual ICollection<SchoolingListLanguangeDto> SchoolingListLanguanges { get; set; }
    }
}
