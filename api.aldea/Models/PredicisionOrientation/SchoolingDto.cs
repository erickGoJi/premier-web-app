using api.premier.Models.Catalogos;
using api.premier.Models.DependentInformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PredicisionOrientation
{
    public class SchoolingDto
    {
        public int Id { get; set; }
        public int? RelationshipId { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public DateTime? Birth { get; set; }
        public int? Age { get; set; }
        public int? Sex { get; set; }
        public int? LanguagesSpoken { get; set; }
        public int? Nationality { get; set; }
        public int? CurrentGrade { get; set; }
        public string Comments { get; set; }
        public int? PredecisionOrientationId { get; set; }
        public bool? Active { get; set; }

        public virtual CatGradeSchoolingDto CurrentGradeNavigation { get; set; }
        public virtual CatLanguagesDto LanguagesSpokenNavigation { get; set; }
        public virtual CatCountryDto NationalityNavigation { get; set; }
        public virtual PredecisionOrientationDto PredecisionOrientation { get; set; }
        public virtual DependentInformationDto Relationship { get; set; }
    }
}
