using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PropertyReport
{
    public class InspectionDto
    {
        public int Id { get; set; }
        public int? HousingList { get; set; }
        public DateTime InitialInspectionDate { get; set; }
        public DateTime? FinalInspectionDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? IdServiceDetail { get; set; }
        public int? PropertySection { get; set; }

        public string InspecDetails { get; set; }

        public int? inspectType { get; set; }


        public int? GroupIrId { get; set; }

        public int? IdStatus { get; set; }


        public virtual ICollection<AttendeeInspecDto> AttendeeInspecs { get; set; }
        public virtual ICollection<PhotosInspecDto> PhotosInspecs { get; set; }

        //public virtual HousingList HousingListNavigation { get; set; }
    }
}
