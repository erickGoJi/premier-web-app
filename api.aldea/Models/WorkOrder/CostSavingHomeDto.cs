using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class CostSavingHomeDto
    {
        public int Id { get; set; }
        public int? HousingList { get; set; }
        public string CostType { get; set; }
        public string CostSavings { get; set; }
        public string CostDesc { get; set; }
        public int? Currency { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? IdServiceDetail { get; set; }

        public int? GroupCostSavingId { get; set; }

        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        //public virtual HousingList HousingListNavigation { get; set; }
    }
}
