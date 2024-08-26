using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Catalogos
{
    public class CatCoordinatorDto
    {
        public int Id { get; set; }
        public string Coordinator { get; set; }
        public int? CoordinatorTypeId { get; set; }
    }
}
