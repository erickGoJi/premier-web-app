using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Departure
{
    public class DepartureDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
        public int StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public bool? PropertyDeparture { get; set; }
        public int? PropertyId { get; set; }
        public DateTime? EstimatedDepartureDate { get; set; }
        public DateTime? ActualDepartureDate { get; set; }
        public DateTime? LeaseTermin { get; set; }
        public DateTime? PickupDate { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool? BankAccountClosing { get; set; }
        public DateTime? BankAccountClosingDate { get; set; }
        public bool? ClubMembershipCancellation { get; set; }
        public DateTime? ClubMembershipCancellationDate { get; set; }
        public bool? UtilityDisconnection { get; set; }
        public DateTime? UtilityDisconnectionDate { get; set; }
        public bool? Other { get; set; }
        public string OtherText { get; set; }
        public DateTime? OtherDate { get; set; }
        public bool? PropertySwitch { get; set; }

        public bool? PermissionCreditSecurityDeposit { get; set; }
        public int? ReturnSecurityDepositTo { get; set; }

        public DateTime? DepartureNotificationLandlord { get; set; }

        public virtual CatStatusDto Status { get; set; }
        public virtual ICollection<CommentDepartureDto> CommentDepartures { get; set; }
        public virtual ICollection<DepartureAssistanceWithDto> DepartureAssistanceWiths { get; set; }
        public virtual ICollection<DocumentDepartureDto> DocumentDepartures { get; set; }
        public virtual ICollection<ReminderDepartureDto> ReminderDepartures { get; set; }
       }
    public partial class DepartureSelectDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
        public int StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public bool? PropertyDeparture { get; set; }
        public int? PropertyId { get; set; }
        public DateTime? LeaseStartDate { get; set; }
        public DateTime? LeaseEndDate { get; set; }
        public decimal? ListRentPrice { get; set; }
        public int? CurrencyListRentPrice { get; set; }
        public decimal? FinalRentPrice { get; set; }
        public int? CurrencyFinalRentPrice { get; set; }
        public bool? DiplomaticClause { get; set; }
        public int? EarlyTerminationNotification { get; set; }
        public DateTime? PaymentsDate { get; set; }
        public int? Penalty { get; set; }
        public decimal? Amount { get; set; }
        public int? CurrencyId { get; set; }
        public bool? PermissionCreditSecurityDeposit { get; set; }
        public int? ReturnSecurityDepositTo { get; set; }
        public DateTime? EstimatedDepartureDate { get; set; }
        public DateTime? ActualDepartureDate { get; set; }
        public DateTime? LeaseTermin { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? InitialInspectionDate { get; set; }
        public DateTime? FinalInspectionDate { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool? BankAccountClosing { get; set; }
        public DateTime? BankAccountClosingDate { get; set; }
        public bool? ClubMembershipCancellation { get; set; }
        public DateTime? ClubMembershipCancellationDate { get; set; }
        public bool? UtilityDisconnection { get; set; }
        public DateTime? UtilityDisconnectionDate { get; set; }
        public bool? Other { get; set; }
        public string OtherText { get; set; }
        public DateTime? OtherDate { get; set; }
        public bool? PropertySwitch { get; set; }

        public virtual CatStatusWorkOrderDto Status { get; set; }

        public DateTime? DepartureNotificationLandlord { get; set; }


        //public virtual WorkOrderService WorkOrderServices { get; set; }
        public virtual ICollection<CommentDepartureSelectDto> CommentDepartures { get; set; }
        public virtual ICollection<DepartureAssistanceWithDto> DepartureAssistanceWiths { get; set; }
        public virtual ICollection<DocumentDepartureDto> DocumentDepartures { get; set; }
        public virtual ICollection<ExtensionDepartureDto> ExtensionDepartures { get; set; }
        public virtual ICollection<ReminderDepartureDto> ReminderDepartures { get; set; }
        }
}
