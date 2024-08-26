using System;

namespace api.premier.Models.PropertyManagement
{
    public class CommentPropertyManagementDto
    {
        public int Id { get; set; }
        public int? PropertyManagement { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual UserDto CreatedByNavigation { get; set; }
        public virtual PropertyManagementDto PropertyManagementNavigation { get; set; }
    }
}