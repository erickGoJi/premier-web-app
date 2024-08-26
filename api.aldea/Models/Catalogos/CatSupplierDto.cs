using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Catalogos
{
    public class CatSupplierDto
    {
        public int Id { get; set; }
        public string Supplier { get; set; }
        public int? UserId { get; set; }
        public int? SupplierType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
