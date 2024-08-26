using System;
using biz.premier.Entities;

namespace api.premier.Models.LeaseRenewal
{
    public class CommentLeaseRenewalDto
    {
        public int Id { get; set; }
        public int? LeaseRenewal { get; set; }
        public string Comment { get; set; }
        public int? CreationBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual UserDto CreationByNavigation { get; set; }
        public virtual LeaseRenewalDto LeaseRenewalNavigation { get; set; }
    }
}