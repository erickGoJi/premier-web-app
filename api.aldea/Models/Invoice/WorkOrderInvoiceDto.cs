using api.premier.Models.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Invoice
{
    public class ServiceInvoiceDto
    {
        public int Id { get; set; }
        public int? Invoice { get; set; }
        public int? WorkOrder { get; set; }
        public int? Service { get; set; }
        public int? ServiceLine { get; set; }
        public int? TypeService { get; set; }
        public int? Status { get; set; }
        public bool? ToInvoice { get; set; }
        public string HourInvoice { get; set; }
        public decimal? AmountToInvoice { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        //public virtual InvoiceDto InvoiceNavigation { get; set; }
    }
}
