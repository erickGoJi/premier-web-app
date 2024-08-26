using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Report
{
    public class ColumnDto
    {
        public int Id { get; set; }
        public int? Report { get; set; }
        public int? Columns { get; set; }
        public int? Orden { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class ColumnSelectDto
    {
        public int Id { get; set; }
        public int? Report { get; set; }
        public int? Columns { get; set; }
        public int? Orden { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual CatColumnsReport ColumnsNavigation { get; set; }
        public virtual ReportDto ReportNavigation { get; set; }
    }
}
