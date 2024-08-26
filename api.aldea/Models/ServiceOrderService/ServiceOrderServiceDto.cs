using api.premier.Models.AssignedServiceSuplier;
using api.premier.Models.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ServiceOrderService
{
    public class WorkOrderServiceDto
    {
        public int Id { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual ICollection<StandaloneServiceWorkOrderDto> StandaloneServiceWorkOrders { get; set; }
        public virtual ICollection<BundledServiceDto> BundledServices { get; set; }

    }
}
