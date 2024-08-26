using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.Models.Catalogue;
using api.premier.Models.Country;
using biz.premier.Entities;

namespace api.premier.Models.Catalogos
{
    public class CatCountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? IdCurrency { get; set; }
        public int? IdLenguage { get; set; }
        public string Sortname { get; set; }
        public string Phonecode { get; set; }
        public string Flag { get; set; }
        public string Nationality { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<CountryDocumentDto> CountryDocuments { get; set; }
        public virtual ICollection<CatCityDto> CatCities { get; set; }
        public virtual ICollection<CountryLeaderDto> CountryLeaders { get; set; }
    }
}
