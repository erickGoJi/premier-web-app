using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestPayment
{
    public class RequestPaymentDto
    {
        public int Id { get; set; }
        public int? WorkOrderServicesId { get; set; }
        public string Title { get; set; }
        public int? Status { get; set; }
        public int? Country { get; set; }
        public int? City { get; set; }
        public string Description { get; set; }
        public bool? Urgent { get; set; }
        public bool? Recurrence { get; set; }
        public DateTime? FundingRequestDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<CommentRequestPaymentDto> CommentRequestPayments { get; set; }
        public virtual ICollection<DocumentRequestPaymentDto> DocumentRequestPayments { get; set; }
        public virtual ICollection<PaymentDto> Payments { get; set; }
        public virtual ICollection<RecurrenceRequestPaymentDto> RecurrenceRequestPayments { get; set; }
    }
    public class RequestPaymentSelectDto
    {
        public int Id { get; set; }
        public int? WorkOrderServicesId { get; set; }
        public string Title { get; set; }
        public int? Status { get; set; }
        public int? Country { get; set; }
        public int? City { get; set; }
        public string Description { get; set; }
        public bool? Urgent { get; set; }
        public bool? Recurrence { get; set; }
        public DateTime? FundingRequestDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual CatCityDto CityNavigation { get; set; }
        public virtual CatCountryDto CountryNavigation { get; set; }
        public virtual CatRequestPaymentStatusDto StatusNavigation { get; set; }
        public virtual ICollection<CommentRequestPaymentSelectDto> CommentRequestPayments { get; set; }
        public virtual ICollection<DocumentRequestPaymentDto> DocumentRequestPayments { get; set; }
    }
}
