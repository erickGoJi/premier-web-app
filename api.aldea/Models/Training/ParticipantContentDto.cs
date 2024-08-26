using System;
using System.Collections.Generic;
using biz.premier.Entities;

namespace api.premier.Models.Training
{
    public class ParticipantContentDto
    {
        public int Id { get; set; }
        public int Participant { get; set; }
        public int Content { get; set; }
        public bool Complete { get; set; }
        public int? Score { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        public virtual ICollection<ParticipantEvaluationDto> ParticipantEvaluations { get; set; }
    }
}