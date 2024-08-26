using System;

namespace api.premier.Models.Catalogue
{
    public class CatPaymentRecurrenceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}