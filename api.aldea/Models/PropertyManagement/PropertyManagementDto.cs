using System;
using System.Collections.Generic;

namespace api.premier.Models.PropertyManagement
{
    public class PropertyManagementDto
    {
        public int Id { get; set; }
        public int? WorkOrderServices { get; set; }
        public bool? Coordination { get; set; }
        public DateTime? AuthoDate { get; set; }
        public DateTime? AuthoAcceptanceDate { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? StatusId { get; set; }
        public string PropertAddress { get; set; }
        public int? Library { get; set; }
        public bool? Vacant { get; set; }
        public bool? Rented { get; set; }
        public string Marketing { get; set; }
        public string Links { get; set; }
        public DateTime? UpcommingVisit { get; set; }
        public int? CountVisit { get; set; }
        public string OfferReceived { get; set; }
        public string Submit { get; set; }
        public int? OfferApprovedTenantSelected { get; set; }
        public string LeaseNegotiation { get; set; }
        public string SendDraft { get; set; }
        public string Comments { get; set; }
        public string SignatureCoordination { get; set; }
        public int? LibraryPaymentCoordination { get; set; }
        public string TenancySupport { get; set; }
        public string CommentsQuestion { get; set; }
        public DateTime? LastPropertyInspection { get; set; }
        public string Report { get; set; }
        public DateTime? NextPropertyInspection { get; set; }
        public int? ItemOngoingMaintanance { get; set; }
        public int? ProviderVisit { get; set; }
        public int? ProviderPayment { get; set; }
        public int? BillType { get; set; }
        public string CommentBill { get; set; }
        public int? ProviderPaymentBill { get; set; }
        public string CommentMail { get; set; }
        public int? Severity { get; set; }
        public string DescriptionReportIssue { get; set; }
        public int? StatusReportIssue { get; set; }
        public DateTime? QuoteApproval { get; set; }

        // public virtual WorkOrderService WorkOrderServicesNavigation { get; set; }
        public virtual ICollection<CommentPropertyManagementDto> CommentPropertyManagements { get; set; }
        public virtual ICollection<DocumentPropertyManagementDto> DocumentPropertyManagements { get; set; }
        public virtual ICollection<DocumentReportIssuePropertyManagementDto> DocumentReportIssuePropertyManagements { get; set; }
        public virtual ICollection<PhotoBillPropertyManagementDto> PhotoBillPropertyManagements { get; set; }
        public virtual ICollection<PhotoInspectionPropertyManagementDto> PhotoInspectionPropertyManagements { get; set; }
        public virtual ICollection<PhotoMailPropertyManagementDto> PhotoMailPropertyManagements { get; set; }
        public virtual ICollection<PhotoPropertyManagementDto> PhotoPropertyManagements { get; set; }
        public virtual ICollection<PhotoReportIssuePropertyManagementDto> PhotoReportIssuePropertyManagements { get; set; }
        public virtual ICollection<ReminderPropertyManagementDto> ReminderPropertyManagements { get; set; }
        public virtual ICollection<VisitReportPropertyManagementDto> VisitReportPropertyManagements { get; set; }
    }
}