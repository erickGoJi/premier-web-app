using System;
using System.Collections.Generic;
using biz.premier.Entities;

namespace api.premier.Models.Training
{
    public class ContentDto
    {
        public int Id { get; set; }
        public int Training { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public int Order { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual trainingDto TrainingNavigation { get; set; }
        // public virtual CatContentType TypeNavigation { get; set; }
        public virtual ICollection<EvaluationDto> Evaluations { get; set; }
        public virtual ICollection<ThemeDto> Themes { get; set; }
    }
}