using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Service
{
    public class ServiceCountryDto
    {
        public int Id { get; set; }
        public int? Service { get; set; }
        public int[] Countries { get; set; }
        public int? Country { get; set; }
        public string ScopeDescription { get; set; }
        public string ScopeTitle { get; set; }
        public bool? ScopeDocuments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<DocumentServiceCountryDto> DocumentServiceCountries { get; set; }
        public virtual ICollection<ServiceCountryLeaderDto> ServiceCountryLeaders { get; set; }
    }
    public class ServiceCountrySelectDto
    {
        public int Id { get; set; }
        public int? Service { get; set; }
        public int? Country { get; set; }
        public string ScopeDescription { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatCountryDto CountryNavigation { get; set; }
        public virtual ICollection<DocumentServiceCountryDto> DocumentServiceCountries { get; set; }
        public virtual ICollection<ServiceCountryLeaderSelectDto> ServiceCountryLeaders { get; set; }
    }
}
