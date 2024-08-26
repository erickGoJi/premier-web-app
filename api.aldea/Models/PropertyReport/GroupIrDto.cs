using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PropertyReport
{
    public class GroupIrDto
    {
        
        public int Id { get; set; }
        public bool? Visible { get; set; }
        public int? IdServiceDetail { get; set; }
        public int? HousingListId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ServiceNumber { get; set; }
        public string ServiceTypeText { get; set; }
        public string WorkOrderText { get; set; }

        public int? WorkOrderServicesId { get; set; }
        public int? CatCategoryId { get; set; }
        public string? CatCategoryText { get; set; }
        public DateTime? DateCreatedInApp { get; set; }
        public bool? CreatedInApp { get; set; }
        public int? IdStatus { get; internal set; }

        public virtual ICollection<InspectionDto> Inspections { get; set; }
        public virtual ICollection<RepairDto> Repairs { get; set; }
       
    }
}
