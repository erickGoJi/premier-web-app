using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.DocumentManagement
{
    public class DocumentManagementDto
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
        public int? HostCityId { get; set; }
        public int? ServiceId { get; set; }
        public int? DocumentTypeId { get; set; }
        public int? SupplierId { get; set; }
        public string ProjectFee { get; set; }
        public DateTime? DocumentCollectionStartDate { get; set; }
        public DateTime? DocumentDeliveryDate { get; set; }
        public DateTime? DocumentExpirationDate { get; set; }
        public DateTime? DocumentCollectionCompletionDate { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public DateTime? ApplicationSubmissionDate { get; set; }
        public DateTime? ApplicationApprovalDate { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual ICollection<CommentDocumentManagementDto> CommentDocumentManagements { get; set; }
        public virtual ICollection<DocumentDocumentManagementDto> DocumentDocumentManagements { get; set; }
        public virtual ICollection<ReminderDocumentManagementDto> ReminderDocumentManagements { get; set; }
    }

    public class DocumentManagementSelectDto
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
        public int? HostCityId { get; set; }
        public int? ServiceId { get; set; }
        public int? DocumentTypeId { get; set; }
        public int? SupplierId { get; set; }
        public string ProjectFee { get; set; }
        public DateTime? DocumentCollectionStartDate { get; set; }
        public DateTime? DocumentDeliveryDate { get; set; }
        public DateTime? DocumentExpirationDate { get; set; }
        public DateTime? DocumentCollectionCompletionDate { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public DateTime? ApplicationSubmissionDate { get; set; }
        public DateTime? ApplicationApprovalDate { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual ICollection<CommentDocumentManagementDto> CommentDocumentManagements { get; set; }
        public virtual ICollection<DocumentDocumentManagementSelectDto> DocumentDocumentManagements { get; set; }
        public virtual ICollection<ReminderDocumentManagementDto> ReminderDocumentManagements { get; set; }
    }
}
