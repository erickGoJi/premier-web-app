using api.premier.Models.PackageServices;
using api.premier.Models.ServiceOrderService;
using api.premier.Models.StandaloneServices;
using api.premier.Models.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ServiceOrder
{
    public class WorkOrderDto
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ServiceLineId { get; set; }
        public int? ServiceRecordId { get; set; }
    }
    public class WorkOrderInsertDto
    {
        public int Id { get; set; }
        public string NumberWorkOrder { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? ServiceLineId { get; set; }
        public int? ServiceRecordId { get; set; }
        public int? StatusId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<BundledServicesWorkOrderDto> BundledServicesWorkOrders { get; set; }
        public virtual ICollection<StandaloneServiceWorkOrderDto> StandaloneServiceWorkOrders { get; set; }
    }
}
