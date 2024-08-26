using System;

namespace api.premier.Models.Catalogs
{
    public class CatCatalogDto
    {
        public int Id { get; set; }
        public string Catalog { get; set; }
        public string Description { get; set; }
        public int? Type { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}