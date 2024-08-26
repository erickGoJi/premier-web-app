using System;

namespace api.premier.Models.TenancyManagement
{
    public class CatEventDto
    {
        public int Id { get; set; }
        public string Event { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}