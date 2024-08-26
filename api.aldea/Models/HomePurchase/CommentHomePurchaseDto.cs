using System;
using biz.premier.Entities;

namespace api.premier.Models.HomePurchase
{
    public class CommentHomePurchaseDto
    {
        public int Id { get; set; }
        public int? HomePurchase { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual UserDto CreatedByNavigation { get; set; }
        public virtual HomePurchaseDto HomePurchaseNavigation { get; set; }
    }
}