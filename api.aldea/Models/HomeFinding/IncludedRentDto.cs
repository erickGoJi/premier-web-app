using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class IncludedRentDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public string Expense { get; set; }
        public string Amount { get; set; }
        public DateTime? Currency { get; set; }
        public bool? Included { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual HomeFindingDto HomeFinding { get; set; }
    }
}
