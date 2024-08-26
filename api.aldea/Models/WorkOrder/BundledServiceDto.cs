using api.premier.Models.ServiceOrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class BundledServiceDto
    {
        public int Id { get; set; }
        public string ServiceNumber { get; set; }
        public int? BundledServiceOrderId { get; set; }
        public int? DeliveredTo { get; set; }
        public int? DeliveringIn { get; set; }
        public int? ServiceId { get; set; }
        public int? ServiceTypeId { get; set; }
        public int? CategoryId { get; set; }
        public string Location { get; set; }
        public DateTime? Autho { get; set; }
        public DateTime? Acceptance { get; set; }
        public int? StatusId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? WorkServicesId { get; set; }
        public bool? InvoiceSupplier { get; set; }
        public string BillingHour { get; set; }

        public virtual BundledServicesWorkOrderDto BundledServiceOrder { get; set; }
        //public virtual CatCategory Category { get; set; }
        //public virtual DependentInformation DeliveredToNavigation { get; set; }
        //public virtual CatDeliviredIn DeliveringInNavigation { get; set; }
        //public virtual CatService Service { get; set; }
        //public virtual CatTypeService ServiceType { get; set; }
        //public virtual ServiceOrderServicesStatus Status { get; set; }
        public virtual WorkOrderServiceDto WorkServices { get; set; }
    }
}
