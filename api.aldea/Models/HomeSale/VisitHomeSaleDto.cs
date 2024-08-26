using System;

namespace api.premier.Models.HomeSale
{
    public class VisitHomeSaleDto
    {
        public int Id { get; set; }
        public int? HomeSale { get; set; }
        public DateTime? Date { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}