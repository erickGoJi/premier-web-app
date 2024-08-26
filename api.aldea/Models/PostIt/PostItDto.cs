using System;

namespace api.premier.Models.PostIt
{
    public class PostItDto
    {
        public int Id { get; set; }
        public string Post { get; set; }
        public string Color { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}