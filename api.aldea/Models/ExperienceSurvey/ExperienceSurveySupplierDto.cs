using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ExperienceSurvey
{
    public class ExperienceSurveySupplierDto
    {
        public int Id { get; set; }
        public int? ExperienceSurveyId { get; set; }
        public int? SupplierService { get; set; }
        public int? SupplierConsultant { get; set; }
        public int? General { get; set; }
        public int? ServiceQuality { get; set; }
        public int? Punctuality { get; set; }
        public int? Attention { get; set; }
        public int? Professionalism { get; set; }
        public int? Price { get; set; }
        public int? ResponseTime { get; set; }
        public string Feedback { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual CatSupplier ExperienceSurvey { get; set; }
        public virtual ExperienceSurveyDto ExperienceSurveyNavigation { get; set; }
    }

    public class ExperienceSurveySupplierSelectDto
    {
        public int Id { get; set; }
        public int? ExperienceSurveyId { get; set; }
        public int? SupplierService { get; set; }
        public int? SupplierConsultant { get; set; }
        public int? General { get; set; }
        public int? ServiceQuality { get; set; }
        public int? Punctuality { get; set; }
        public int? Attention { get; set; }
        public int? Professionalism { get; set; }
        public int? Price { get; set; }
        public int? ResponseTime { get; set; }
        public string Feedback { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ExperienceSurveyDto ExperienceSurveyNavigation { get; set; }
    }
}
