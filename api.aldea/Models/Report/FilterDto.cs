using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Report
{
    public class FilterDto
    {
        public int Id { get; set; }
        public int? Report { get; set; }
        public int? Filter1 { get; set; }
        public int? Value { get; set; }
        public DateTime? FirstDate { get; set; }
        public DateTime? SecondDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class FilterSelectDto
    {
        public int Id { get; set; }
        public int? Report { get; set; }
        public int? Filter1 { get; set; }
        public int? Value { get; set; }
        public DateTime? FirstDate { get; set; }
        public DateTime? SecondDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual CatFilterReport Filter1Navigation { get; set; }
        public virtual ReportDto ReportNavigation { get; set; }
    }
}
