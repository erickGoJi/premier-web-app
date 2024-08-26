using System;
using System.Collections.Generic;
using biz.premier.Entities;

namespace api.premier.Models.Training
{
    public class EvaluationDto
    {
        public int Id { get; set; }
        public int Content { get; set; }
        public int Element { get; set; }
        public string Text { get; set; }
        public string Question { get; set; }
        public bool Required { get; set; }
        public int Order { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ContentDto ContentNavigation { get; set; }
        // public virtual CatElement ElementNavigation { get; set; }
        public virtual ICollection<AnswerDto> Answers { get; set; }
    }
}