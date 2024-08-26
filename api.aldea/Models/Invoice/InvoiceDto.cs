using api.premier.Models.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public int? ServiceRecord { get; set; }
        public int? InvoiceType { get; set; }
        public int? Consultant { get; set; }
        public string Comments { get; set; }
        public string InoviceNo { get; set; }
        public string PaymentNumber { get; set; }
        public int? MethodPayment { get; set; }
        public int? PaymentId { get; set; }
        public int? Office { get; set; }
        public int? Contact { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }


        //public virtual CatInvoiceTypeDto InvoiceTypeNavigation { get; set; }
        //public virtual CatStatusInvoiceDto StatusNavigation { get; set; }
        public virtual ICollection<AdditionalExpenseDto> AdditionalExpenses { get; set; }
        public virtual ICollection<CommentInvoiceDto> CommentInvoices { get; set; }
        public virtual ICollection<DocumentInvoiceDto> DocumentInvoices { get; set; }
        public virtual ICollection<ServiceInvoiceDto> ServiceInvoices { get; set; }
    }
}
