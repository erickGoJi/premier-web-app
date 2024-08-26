using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.LegalReview
{
    public class LegalReviewDto
    {
        public int Id { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public int ApplicantId { get; set; }
        public int StatusId { get; set; }
        public string Name { get; set; }
        public int? WorkOrderServicesId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? HostCountryId { get; set; }
        public int? BilliableHours { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual ICollection<CommentLegalReviewDto> CommentLegalReviews { get; set; }
        public virtual ICollection<DocumentLegalReviewDto> DocumentLegalReviews { get; set; }
        public virtual ICollection<ReminderLegalReviewDto> ReminderLegalReviews { get; set; }
    }

    public class LegalReviewSelectDto
    {
        public int Id { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public int ApplicantId { get; set; }
        public string ApplicantName { get; set; }
        public string Relationship { get; set; }
        public int StatusId { get; set; }
        public string Name { get; set; }
        public int? WorkOrderServicesId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? HostCountryId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int? BilliableHours { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual ICollection<CommentLegalReviewDto> CommentLegalReviews { get; set; }
        public virtual ICollection<DocumentLegalReviewSelectDto> DocumentLegalReviews { get; set; }
        public virtual ICollection<ReminderLegalReviewDto> ReminderLegalReviews { get; set; }
    }
}
