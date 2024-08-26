using api.premier.Models.Catalogos;
using System.Collections.Generic;

namespace api.premier.Models.ClientPartnerProfile
{
    public class ServiceLocationCountryDto
    {
        public int Id { get; set; }
        public int IdServiceLocation { get; set; }
        public int IdCountry { get; set; }
        public int[] Countries { get; set; }
        public long StandarScopeDocuments { get; set; }
        public string ScopeDescription { get; set; }

        public virtual ICollection<DocumentLocationCountryDto> DocumentLocationCountries { get; set; }
    }
}