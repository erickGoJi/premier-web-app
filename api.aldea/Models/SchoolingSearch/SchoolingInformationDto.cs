using api.premier.Models.Catalogos;
using api.premier.Models.DependentInformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SchoolingSearch
{
    public class SchoolingInformationDto
    {
        public int Id { get; set; }
        public int? SchoolingSearchId { get; set; }
        public int? RelationshipId { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public int? Sex { get; set; }
        public DateTime? Birth { get; set; }
        public int? Age { get; set; }
        public int? Nationality { get; set; }
        public int? CurrentGrade { get; set; }
        public string Comments { get; set; }
        public bool? Active { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatGradeSchoolingDto CurrentGradeNavigation { get; set; }
        public virtual NationalityDto NationalityNavigation { get; set; }
        public virtual DependentInformationDto Relationship { get; set; }
        public virtual SchoolingSearchDto SchoolingSearch { get; set; }
        public virtual CatSexDto SexNavigation { get; set; }
        public virtual ICollection<LanguangeSpokenSchoolingInformationDto> LanguangeSpokenSchoolingInformations { get; set; }
    }

    public class LanguangeSpokenSchoolingInformationDto
    {
        public int SchoolingInformation { get; set; }
        public int LanguageSpoken { get; set; }

        public virtual CatLanguagesDto LanguageSpokenNavigation { get; set; }
        //public virtual SchoolingInformation SchoolingInformationNavigation { get; set; }
    }
}
