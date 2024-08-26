using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Models
{
    public class WorkOrderEstatus
    {
        public int Id { get; set; }
        public StandaloneServiceWorkOrders[] StandaloneServiceWorkOrders { get; set; }
        public BundledServicesWorkOrders[] BundledServicesWorkOrders { get; set; }

    }

    public class StandaloneServiceWorkOrders
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public int DeliveredTo { get; set; }
        public int DeliveringIn { get; set; }
    }

    public class BundledServicesWorkOrders
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public int DeliveredTo { get; set; }
        public int DeliveringIn { get; set; }
    }
}
