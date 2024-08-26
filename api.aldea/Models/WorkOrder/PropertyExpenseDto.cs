using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class PropertyExpenseDto
    {
        public int Id { get; set; }
        public int? ContractDetail { get; set; }
        public string Expense { get; set; }
        public decimal? Amount { get; set; }
        public int? Currency { get; set; }
        public string Recurrence { get; set; }
        public bool? Included { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        public virtual CatDurationDto RecurrenceNavigation { get; set; }
        //public virtual ContractDetailDto ContractDetailNavigation { get; set; }
    }
}
