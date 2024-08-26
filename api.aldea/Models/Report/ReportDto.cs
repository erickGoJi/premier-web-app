using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Report
{
    public class ReportDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<ColumnDto> Columns { get; set; }
        public virtual ICollection<FilterDto> Filters { get; set; }
    }
    public class ReportInsertDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ReportType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
