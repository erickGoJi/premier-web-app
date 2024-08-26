using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Catalogos
{
    public class CatServiceDto
    {
        public int Id { get; set; }
        public string Service { get; set; }
        public int? CategoryId { get; set; }
    }
}
