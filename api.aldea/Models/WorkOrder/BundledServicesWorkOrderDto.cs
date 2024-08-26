using api.premier.Models.ServiceOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class BundledServicesWorkOrderDto
    {
        public BundledServicesWorkOrderDto()
        {
            BundledServices = new HashSet<BundledServiceDto>();
        }

        public int Id { get; set; }
        public int? WorkOrderId { get; set; }
        public string TotalTime { get; set; }
        public string ProjectedFee { get; set; }
        public bool? Package { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual WorkOrderInsertDto WorkOrder { get; set; }
        public virtual ICollection<BundledServiceDto> BundledServices { get; set; }
    }
}
