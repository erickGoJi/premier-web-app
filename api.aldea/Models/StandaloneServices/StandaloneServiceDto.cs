using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.StandaloneServices
{
    public class StandaloneServiceDto
    {
        public int Id { get; set; }
        public int? DeliveredTo { get; set; }
        public int? DeliveringIn { get; set; }
        public string Location { get; set; }
        public DateTime? Acceptance { get; set; }
        public bool? Coordination { get; set; }
        public int? AuthoTime { get; set; }
        public string ProjectedFee { get; set; }
        public int? ServiceOrderId { get; set; }
        public int? ServiceId { get; set; }
    }
}
