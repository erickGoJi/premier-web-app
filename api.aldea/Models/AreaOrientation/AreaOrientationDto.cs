using api.premier.Models.Catalogos;
using api.premier.Models.ServiceOrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AreaOrientation
{
    public class AreaOrientationDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
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

        //public virtual CatStatusDto Status { get; set; }
        //public virtual WorkOrderServiceDto WorkOrderServices { get; set; }
        public virtual ICollection<CommentAreaOrientationDto> CommentAreaOrientations { get; set; }
        public virtual ICollection<DocumentAreaOrientationDto> DocumentAreaOrientations { get; set; }
        //public virtual ICollection<ExtensionAreaOrientationDto> ExtensionAreaOrientations { get; set; }
        public virtual ICollection<ReminderAreaOrientationDto> ReminderAreaOrientations { get; set; }
        public virtual ICollection<SchoolingAreaOrientationDto> SchoolingAreaOrientations { get; set; }
    }

    public class AreaOrientationSelectDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
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

        public virtual CatStatusWorkOrderDto Status { get; set; }
        public virtual WorkOrderServiceDto WorkOrderServices { get; set; }
        public virtual ICollection<CommentAreaOrientationSelectDto> CommentAreaOrientations { get; set; }
        public virtual ICollection<DocumentAreaOrientationSelectDto> DocumentAreaOrientations { get; set; }
        public virtual ICollection<ExtensionAreaOrientationDto> ExtensionAreaOrientations { get; set; }
        public virtual ICollection<ReminderAreaOrientationDto> ReminderAreaOrientations { get; set; }
        public virtual ICollection<SchoolingAreaOrientationDto> SchoolingAreaOrientations { get; set; }
    }
}
