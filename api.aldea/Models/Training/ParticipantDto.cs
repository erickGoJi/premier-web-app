using System;

namespace api.premier.Models.Training
{
    public class ParticipantDto
    {
        public int Id { get; set; }
        public int Training { get; set; }
        public int UserParticipant { get; set; }
        public int? Status { get; set; }
        public DateTime? CompletedDate { get; set; }
        public decimal? Percentage { get; set; }
        public int? Score { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}