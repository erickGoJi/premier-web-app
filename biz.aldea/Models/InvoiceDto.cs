using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Models
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public int? ServiceRecord { get; set; }
        public int? InvoiceType { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
