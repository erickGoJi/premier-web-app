using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Models
{
    public class ServiceRecordSelectCustomDto
    {
        public ServiceRecordSelectCustomDto()
        {
            services = new HashSet<WorkOrder>();
        }
        public int Id { get; set; }
        public DateTime? InitialAutho { get; set; }
        public string client { get; set; }
        public int? clientId { get; set; }
        public string partner { get; set; }
        public int? partnerId { get; set; }
        public bool? status { get; set; }
        public bool? Spoc { get; set; }
        public bool? Vip { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string assigneeName { get; set; }
        public virtual ICollection<WorkOrder> services { get; set; }
        public string coordinator { get; set; }
        public string supplierConsultant { get; set; }
    }
}
