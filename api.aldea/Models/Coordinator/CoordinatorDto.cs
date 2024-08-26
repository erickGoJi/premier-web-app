using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Coordinator
{
    public class CoordinatorDto
    {
        public int Id { get; set; }
        public string Photo { get; set; }
        public int? SupplierType { get; set; }
        public bool? Immigration { get; set; }
        public bool? Relocation { get; set; }
        public string ContactName { get; set; }
        public string Title { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? City { get; set; }
        public string Comments { get; set; }
        public int? UserId { get; set; }
        public int? CoordinatorType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
