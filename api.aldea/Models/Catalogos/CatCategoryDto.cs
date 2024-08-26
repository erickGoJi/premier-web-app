using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Catalogos
{
    public class CatCategoryDto
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public int? SericeLineId { get; set; }
    }
}
