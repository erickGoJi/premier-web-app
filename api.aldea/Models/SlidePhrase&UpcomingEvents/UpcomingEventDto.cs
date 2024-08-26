using System;
using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;

namespace api.premier.Models
{
    public class UpcomingEventDto
    {
        public int Id { get; set; }
        public DateTime? EventDate { get; set; }
        public string Title { get; set; }
        public int? Country { get; set; }
        public int? City { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatCityDto CityNavigation { get; set; }
        public virtual CatCountryDto CountryNavigation { get; set; }
    }
}