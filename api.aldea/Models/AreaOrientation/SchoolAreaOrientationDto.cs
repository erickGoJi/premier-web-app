using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using api.premier.Models.DependentInformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AreaOrientation
{
    public class SchoolAreaOrientationDto
    {
        public int Id { get; set; }
        public int? AreaOrientationId { get; set; }
        public int? Dependent { get; set; }
        public string SchoolName { get; set; }
        public int? Country { get; set; }
        public int? City { get; set; }
        public string Address { get; set; }
        public int? Grade { get; set; }
        public int? Languages { get; set; }
        public string Url { get; set; }
        public bool? Ib { get; set; }
        public bool? Uniform { get; set; }
        public bool? MedicalRecords { get; set; }
        public string Admision { get; set; }
        public decimal? PerMonth { get; set; }
        public decimal? ForEnrollment { get; set; }
        public decimal? FeesEnrollment { get; set; }
        public string AdditionalComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AreaOrientationDto AreaOrientation { get; set; }
        public virtual CatCityDto CityNavigation { get; set; }
        public virtual CatCountryDto CountryNavigation { get; set; }
        public virtual DependentInformationDto DependentNavigation { get; set; }
        public virtual CatGradeSchoolingDto GradeNavigation { get; set; }
        public virtual CatLanguagesDto LanguagesNavigation { get; set; }
    }
}
