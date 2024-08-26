using System;
using biz.premier.Entities;

namespace api.premier.Models.Training
{
    public class AnswerDto
    {
        public int Id { get; set; }
        public int Evaluation { get; set; }
        public string Answer1 { get; set; }
        public bool Correct { get; set; }
        public int Order { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual EvaluationDto EvaluationNavigation { get; set; }
    }
}