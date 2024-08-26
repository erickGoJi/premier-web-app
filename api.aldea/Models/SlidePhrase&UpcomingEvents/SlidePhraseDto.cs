using System;

namespace api.premier.Models
{
    public class SlidePhraseDto
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Extension { get; set; }
        public bool Status { get; set; }
        public string Phrase { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}