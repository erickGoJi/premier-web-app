using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PropertyReport
{
    public class SectionInventoryDto
    {
        public int Id { get; set; }
        public int? PropertyReportSectionId { get; set; }
        public int? Item { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual PropertyReportSectionDto PropertyReportSection { get; set; }
        public virtual ICollection<PhotosInventoryDto> PhotosInventories { get; set; }
    }
}
