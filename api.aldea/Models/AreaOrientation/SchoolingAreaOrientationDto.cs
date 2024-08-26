using api.premier.Models.Catalogos;
using api.premier.Models.DependentInformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AreaOrientation
{
    public class SchoolingAreaOrientationDto
    {
        public int Id { get; set; }
        public int? RelationshipId { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public DateTime? Birth { get; set; }
        public int? Sex { get; set; }
        public int? Age { get; set; }
        public int? Nationality { get; set; }
        public int? CurrentGrade { get; set; }
        public string Comments { get; set; }
        public bool? Active { get; set; }
        public int? AreaOrientationId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AreaOrientationDto AreaOrientation { get; set; }
        public virtual CatGradeSchoolingDto CurrentGradeNavigation { get; set; }
        public virtual CatSexDto SexNavigation { get; set; }
        public virtual CatCountryDto NationalityNavigation { get; set; }
        public virtual DependentInformationDto Relationship { get; set; }
        public virtual ICollection<LanguagesSpokenSchoolingAreaOrientationDto> LanguagesSpokenSchoolingAreaOrientations { get; set; }
    }

    public class LanguagesSpokenSchoolingAreaOrientationDto
    {
        public int Schooling { get; set; }
        public int LanguagesSpoken { get; set; }

        public virtual CatLanguagesDto LanguagesSpokenNavigation { get; set; }
        //public virtual SchoolingAreaOrientation SchoolingNavigation { get; set; }
    }
}
