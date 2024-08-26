using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class CostSavingDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public string Comments { get; set; }

        public virtual HomeFindingDto HomeFinding { get; set; }
        public virtual ICollection<CostSavingDetailDto> CostSavingDetails { get; set; }
    }
}
