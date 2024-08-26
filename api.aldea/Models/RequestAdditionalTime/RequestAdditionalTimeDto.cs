using api.premier.Models.ServiceOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestAdditionalTime
{
    public class RequestAdditionalTimeDto
    {
        public int Id { get; set; }
        public int? WorkOrder { get; set; }
        public int? Service { get; set; }
        public int? RequestTime { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual WorkOrderDto WorkOrderNavigation { get; set; }
    }
}
