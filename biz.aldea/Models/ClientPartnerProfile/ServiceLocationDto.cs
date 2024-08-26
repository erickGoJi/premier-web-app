using System.Collections.Generic;

namespace biz.premier.Models.ClientPartnerProfile
{
    public class ServiceLocationDto
    {
        public int Id { get; set; }
        public int IdClientPartnerProfile { get; set; }
        public int IdServiceLine { get; set; }
        public int IdService { get; set; }
        public string ServicesName { get; set; }
        public string NickName { get; set; }
        public int country_total { get; set; }

        public virtual ICollection<ServiceLocationCountryDto> ServiceLocationCountries { get; set; }
    }
}