using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Country
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Country1 { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int? UpdatedDate { get; set; }

        public virtual ICollection<CountryGalleryDto> CountryGalleries { get; set; }
    }
}
