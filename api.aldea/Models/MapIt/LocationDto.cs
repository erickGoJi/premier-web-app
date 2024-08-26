using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.MapIt
{
    public class LocationDto
    {
        public int Id { get; set; }
        public int? MapItId { get; set; }
        public int? LocationType { get; set; }
        public int? Service { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        //public virtual CatLocationType LocationTypeNavigation { get; set; }
        public virtual MapItDto MapIt { get; set; }
    }
}
