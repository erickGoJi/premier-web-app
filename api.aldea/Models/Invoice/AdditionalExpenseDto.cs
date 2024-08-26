using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Invoice
{
    public class AdditionalExpenseDto
    {
        public int Id { get; set; }
        public int? Invoice { get; set; }
        public bool? Requested { get; set; }
        public string Concept { get; set; }
        public decimal? Amount { get; set; }
        public int? Currency { get; set; }

    }
}
