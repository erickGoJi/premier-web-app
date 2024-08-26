using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Catalogos
{
    public class CatBreedDto
    {
        public int Id { get; set; }
        public string Breed { get; set; }
        public int PetTypeId { get; set; }
        public bool Status { get; set; }
        public virtual CatPetTypeDto PetType { get; set; }

    }
}
