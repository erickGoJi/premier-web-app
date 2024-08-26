using System;

namespace api.premier.Models.Catalogs
{
    public class CatTypeCatalogDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}