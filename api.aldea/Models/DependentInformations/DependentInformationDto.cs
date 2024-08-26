using api.premier.Models.DependentInformationLanguage;
using api.premier.Models.WorkOrder;
using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.DependentInformations
{
    public class DependentInformationDto
    {
        public int Id { get; set; }
        public int RelationshipId { get; set; }
        public string Photo { get; set; }
        public string PhotoExtension { get; set; }
        public string Name { get; set; }
        public DateTime? Birth { get; set; }
        public int? Age { get; set; }
        public int? NationalityId { get; set; }
        public string AditionalComments { get; set; }
        public int AssigneeInformationId { get; set; }
        public int? LanguagesId { get; set; }
        public string IfOther { get; set; }
        public int? CurrentGrade { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? Sex { get; set; }
        public virtual ICollection<LanguageDependentInformationDto> LanguageDependentInformations { get; set; }
        public virtual ICollection<SchoolsListDto> SchoolsList { get; set; }

    }

}
