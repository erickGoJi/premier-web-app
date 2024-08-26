using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestPayment
{
    public class RecurrenceRequestPaymentDto
    {
        public int Id { get; set; }
        public int? RequestPayment { get; set; }
        public int? RepeatEvery { get; set; }
        public int? Period { get; set; }
        public DateTime? StartDate { get; set; }
        public bool? Never { get; set; }
        public bool? Date { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual ICollection<RepeatTheRecurrenceRequestPaymentDto> RepeatTheRecurrenceRequestPayments { get; set; }
    }
    public class RecurrenceRequestPaymentSelectDto
    {
        public int Id { get; set; }
        public int? RequestPayment { get; set; }
        public int? RepeatEvery { get; set; }
        public int? Period { get; set; }
        public DateTime? StartDate { get; set; }
        public bool? Never { get; set; }
        public bool? Date { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatDurationDto PeriodNavigation { get; set; }
        public virtual ICollection<RepeatTheRecurrenceRequestPaymentDto> RepeatTheRecurrenceRequestPayments { get; set; }
    }
}
