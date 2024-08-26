using api.premier.Models.RequestPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PredicisionOrientation
{
    public class PredecisionOrientationDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public int StatusId { get; set; }
        public DateTime? AuthoDateExtension { get; set; }
        public DateTime? AuthoAcceptanceDateExtension { get; set; }
        public string Time { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public bool? Supermarks { get; set; }
        public bool? ShoppingSocialAreas { get; set; }
        public bool? Parks { get; set; }
        public bool? ExtracurricularActivities { get; set; }
        public bool? Housing { get; set; }
        public bool? Schooling { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? Supermarksdate { get; set; }
        public DateTime? ShoppingSocialAreasdate { get; set; }
        public DateTime? Parksdate { get; set; }
        public DateTime? ExtracurricularActivitiesdate { get; set; }
        public DateTime? EmergencyHealthdate { get; set; }
        public DateTime? Otherdate { get; set; }
        public string Othertext { get; set; }
        public bool? Other { get; set; }
        public bool? EmergencyHealth { get; set; }
        public bool? Showschooling { get; set; }
        public bool? Showhousing { get; set; }
        public virtual ICollection<CommentPredecisionOrientationDto> CommentPredecisionOrientations { get; set; }
        public virtual ICollection<DocumentPredecisionOrientationDto> DocumentPredecisionOrientations { get; set; }
        //public virtual ICollection<ExtensionPredecisionOrientationDto> ExtensionPredecisionOrientations { get; set; }
        //public virtual ICollection<HousingPredecisionOrientationDto> HousingPredecisionOrientations { get; set; }
        public virtual ICollection<ReminderPredecisionOrientationDto> ReminderPredecisionOrientations { get; set; }
        //public virtual ICollection<SchoolPredecisionOrientationDto> SchoolPredecisionOrientations { get; set; }
        public virtual ICollection<SchoolingDto> Schoolings { get; set; }
    }

    public class PredecisionOrientationSelectDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public int StatusId { get; set; }
        public DateTime? AuthoDateExtension { get; set; }
        public DateTime? AuthoAcceptanceDateExtension { get; set; }
        public string Time { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public bool? Supermarks { get; set; }
        public bool? ShoppingSocialAreas { get; set; }
        public bool? Parks { get; set; }
        public bool? ExtracurricularActivities { get; set; }
        public bool? Housing { get; set; }
        public bool? Schooling { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? Supermarksdate { get; set; }
        public DateTime? ShoppingSocialAreasdate { get; set; }
        public DateTime? Parksdate { get; set; }
        public DateTime? ExtracurricularActivitiesdate { get; set; }
        public DateTime? EmergencyHealthdate { get; set; }
        public DateTime? Otherdate { get; set; }
        public string Othertext { get; set; }
        public bool? Other { get; set; }
        public bool? EmergencyHealth { get; set; }
        public bool? Showschooling { get; set; }
        public bool? Showhousing { get; set; }
        public virtual ICollection<CommentPredecisionOrientationSelectDto> CommentPredecisionOrientations { get; set; }
        public virtual ICollection<DocumentPredecisionOrientationSelectDto> DocumentPredecisionOrientations { get; set; }
        public virtual ICollection<ExtensionPredecisionOrientationDto> ExtensionPredecisionOrientations { get; set; }
        //public virtual ICollection<HousingPredecisionOrientationDto> HousingPredecisionOrientations { get; set; }
        public virtual ICollection<ReminderPredecisionOrientationDto> ReminderPredecisionOrientations { get; set; }
        //public virtual ICollection<SchoolPredecisionOrientationDto> SchoolPredecisionOrientations { get; set; }
        public virtual ICollection<SchoolingDto> Schoolings { get; set; }
    }
}
