using System;

namespace api.premier.Models.PropertyManagement
{
    public class CatLibraryDto
    {
        public int Id { get; set; }
        public string Library { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}