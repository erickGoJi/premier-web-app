using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class CostSavingDetailDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public string CostType { get; set; }
        public string CostSavings { get; set; }
        public string CostSavingsCurrency { get; set; }
        public string Comments { get; set; }

        public virtual HomeFindingDto HomeFinding { get; set; }
        //public virtual CatCurrencyDto CostSavingsCurrencyNavigation { get; set; }
        //public virtual CatCostTypeDto CostTypeNavigation { get; set; }
    }
}
