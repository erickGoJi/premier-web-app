using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class GroupCostSavingDto
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

        public virtual ICollection<CostSavingHomeDto> CostSavingHomes { get; set; }
    }
}
