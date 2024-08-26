using System;

namespace api.premier.Models.Training
{
    public class CatContentTypeDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}