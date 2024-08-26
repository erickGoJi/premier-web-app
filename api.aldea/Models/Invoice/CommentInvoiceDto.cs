using api.premier.Models.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Invoice
{
    public class CommentInvoiceDto
    {
        public int Id { get; set; }
        public int? Invoice { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual UserDto CreatedByNavigation { get; set; }
        //public virtual InvoiceDto InvoiceNavigation { get; set; }
    }
}
