using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PropertyReport
{
    public class PhotosInspecDto
    {
        public int PropertyReportSectionId { get; set; }
        public string PhotoName { get; set; }
        public string Photo { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int Id { get; set; }
        public int? Inspection { get; set; }

        public string PhotoExtension { get; set; }

        public virtual InspectionDto InspectionNavigation { get; set; }
    }
}
