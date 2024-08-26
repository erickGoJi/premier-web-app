using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ExperienceSurvey
{
    public class ExperienceSurveyDto
    {
        public int Id { get; set; }
        public int? ServiceRecord { get; set; }
        public int? WorkOrderServicesId { get; set; }
        public int? ServiceLine { get; set; }
        public bool? CompleteServiceRecord { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Name { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual ServiceRecord ServiceRecordNavigation { get; set; }
        public virtual ICollection<ExperienceSurveySupplierDto> ExperienceSurveySuppliers { get; set; }
    }

    public class ExperienceSurveySelectDto
    {
        public int Id { get; set; }
        public int? ServiceRecord { get; set; }
        public int? WorkOrderServicesId { get; set; }
        public int? ServiceLine { get; set; }
        public bool? CompleteServiceRecord { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Name { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual ServiceRecord ServiceRecordNavigation { get; set; }
        public virtual CatServiceLineDto ServiceLineNavigation { get; set; }
        public virtual ICollection<ExperienceSurveySupplierSelectDto> ExperienceSurveySuppliers { get; set; }
    }
}
