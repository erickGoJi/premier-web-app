using api.premier.Models.Catalogos;
using System.Collections.Generic;

namespace api.premier.Models.ClientPartnerProfile
{
    public class ServiceLocationDto
    {
        public int Id { get; set; }
        public int IdClientPartnerProfile { get; set; }
        public int IdServiceLine { get; set; }
        public int IdService { get; set; }
        public int[] Services { get; set; }
        public string NickName { get; set; }

        public virtual ICollection<ServiceLocationCountryDto> ServiceLocationCountries { get; set; }
    }
}