using System;

namespace api.premier.Models.Catalogue
{
    public class CatIndustryDto
    {
        public int Id { get; set; }
        public string Industry { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}