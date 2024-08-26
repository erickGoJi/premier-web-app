using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestPayment
{
    public class ConceptDto
    {
        public int Id { get; set; }
        public int? Payment { get; set; }
        public string Concept1 { get; set; }
        public decimal? Amount { get; set; }
        public int? Currency { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
