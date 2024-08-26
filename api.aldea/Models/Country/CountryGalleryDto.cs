using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Country
{
    public class CountryGalleryDto
    {
        public int Id { get; set; }
        public int? CountryId { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CountryDto Country { get; set; }
    }
}
