using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Departure
{
    public class DepartureCostSavingDto
    {
        public int Id { get; set; }
        public int? DepartureId { get; set; }
        public string CostType { get; set; }
        public string CostSavings { get; set; }
        public int? CostSavingsCurrency { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatCurrencyDto CostSavingsCurrencyNavigation { get; set; }
        public virtual DepartureDto Departure { get; set; }
    }
}
