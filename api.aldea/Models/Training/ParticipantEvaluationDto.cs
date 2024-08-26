using System;
using System.Collections.Generic;
using biz.premier.Entities;

namespace api.premier.Models.Training
{
    public class ParticipantEvaluationDto
    {
        public int Id { get; set; }
        public int? ParticipantContent { get; set; }
        public int? Question { get; set; }
        public int? Answer { get; set; }
        public bool? Correct { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}