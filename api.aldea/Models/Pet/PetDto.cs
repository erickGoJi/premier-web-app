using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Pet
{
    public class PetDto
    {
        public int Id { get; set; }
        public int? PetTypeId { get; set; }
        public string Name { get; set; }
        public string BreedId { get; set; }
        public int? Age { get; set; }        
        public DateTime? BirthDate { get; set; }
        public int? SizeId { get; set; }
        public decimal? Weight { get; set; }
        public int? WeightMeasuresId { get; set; }
        public string Photo { get; set; }

        public int AssigneeInformationId { get; set; }
        public string Color { get; set; }
    }
}
