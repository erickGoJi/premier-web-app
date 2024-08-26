using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class HousingReportDto
    {
        public int Id { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? WorkOrder { get; set; }
        public int? ServiceId { get; set; }
        public int? SampleProperty { get; set; }
        public int? HousingList { get; set; }
        public string ReportDetails { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
