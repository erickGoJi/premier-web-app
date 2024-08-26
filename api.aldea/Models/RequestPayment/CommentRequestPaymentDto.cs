using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestPayment
{
    public class CommentRequestPaymentDto
    {
        public int Id { get; set; }
        public int? RequestPaymentId { get; set; }
        public string Reply { get; set; }
        public int? UserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual RequestPaymentDto RequestPayment { get; set; }
    }
    public class CommentRequestPaymentSelectDto
    {
        public int Id { get; set; }
        public int? RequestPaymentId { get; set; }
        public string Reply { get; set; }
        public int? UserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual RequestPaymentDto RequestPayment { get; set; }
        public virtual UserDto User { get; set; }
    }
}
