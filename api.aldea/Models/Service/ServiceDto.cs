using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Service
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public int? Service1 { get; set; }
        public int? ServiceLine { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual ICollection<ServiceCountryDto> ServiceCountries { get; set; }

    }

    public class ServiceSelectDto
    {
        public int Id { get; set; }
        public int? Service1 { get; set; }
        public int? ServiceLine { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatServiceDto Service1Navigation { get; set; }
        public virtual CatServiceLineDto ServiceLineNavigation { get; set; }
        public virtual ICollection<ServiceCountryDto> ServiceCountries { get; set; }
    }
}
