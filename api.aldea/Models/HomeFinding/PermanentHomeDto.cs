using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class PermanentHomeDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public int? PorpertyNo { get; set; }
        public int? PropertyType { get; set; }
        public int? Beds { get; set; }
        public int? Baths { get; set; }
        public string Address { get; set; }
        public string WebSite { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual HomeFindingDto HomeFinding { get; set; }
        //public virtual CatPropertyDto PorpertyNoNavigation { get; set; }
        //public virtual CatPropertyTypeHousing PropertyTypeNavigation { get; set; }
    }
}
