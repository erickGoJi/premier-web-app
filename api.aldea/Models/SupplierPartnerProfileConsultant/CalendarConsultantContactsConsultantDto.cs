using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileConsultant
{
    public class CalendarConsultantContactsConsultantDto
    {
        public int Id { get; set; }
        public int? ConsultantContactsConsultant { get; set; }
        public int? Day { get; set; }
        public DateTime? Date { get; set; }
        public string HourStart { get; set; }
        public string HourEnd { get; set; }
        public bool? Available { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
