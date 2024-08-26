using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Catalogos
{
    public class CatRelationshipDto
    {
        public int Id { get; set; }
        public string Relationship { get; set; }
        public bool Status { get; set; }
    }
}
