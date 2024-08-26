using System;
using System.Collections.Generic;
using biz.premier.Entities;

namespace api.premier.Models.Training
{
    public class CatElementDto
    {
        public int Id { get; set; }
        public string Element { get; set; }
        public int? Type { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // public virtual ICollection<Evaluation> Evaluations { get; set; }
        // public virtual ICollection<Theme> Themes { get; set; }
    }
}