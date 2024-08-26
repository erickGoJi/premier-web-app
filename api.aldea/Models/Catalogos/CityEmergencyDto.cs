using System.Collections.Generic;

namespace api.premier.Models.Catalogue
{
    public class CityEmergencyDto
    {
        public int Id { get; set; }
        public int IdCity { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public int? TypeId { get; set; }
        public virtual ICollection<PhotoCityEmergencyDto> PhotoCityEmergencies { get; set; }
    }
}