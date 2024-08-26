using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RentalFurnitureCoordination
{
    public class RentalFurnitureCoordinationDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
        public int? DeliveredTo { get; set; }
        public int StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? TotalTimeAllowed { get; set; }
        public int? TotalTimeAllowedId { get; set; }
        public decimal? Budget { get; set; }
        public int? Currency { get; set; }
        public int? Baths { get; set; }
        public int? Beds { get; set; }
        public int? FamilyMembers { get; set; }
        public int? PaymentResponsibility { get; set; }
        public int? SupplierPartner { get; set; }
        public int? MainContact { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ScheduledPickUpDate { get; set; }
        public int? TotalDays { get; set; }
        public int? DeliverTo { get; set; }
        public string PropertyAddress { get; set; }
        public decimal? RentalAmount { get; set; }
        public int? CurrencyExtension { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public decimal? PaymentDue { get; set; }
        public int? ReturnSecurityDeposit { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? IdCatPaymentmethods { get; set; }
        public decimal? RentalOngoingAmount { get; set; }


        public virtual ICollection<CommentRentalFurnitureCoordinationDto> CommentRentalFurnitureCoordinations { get; set; }
        public virtual ICollection<DocumentRentalFurnitureCoordinationDto> DocumentRentalFurnitureCoordinations { get; set; }
        public virtual ICollection<PaymentsRentalFurnitureCoordinationDto> PaymentsRentalFurnitureCoordinations { get; set; }
        public virtual ICollection<ReminderRentalFurnitureCoordinationDto> ReminderRentalFurnitureCoordinations { get; set; }
        public virtual ICollection<StayExtensionRentalFurnitureCoordinationDto> StayExtensionRentalFurnitureCoordinations { get; set; }
        public virtual ICollection<PaymentRentalDto> PaymentRentals { get; set; }
    }
    public class RentalFurnitureCoordinationSelectDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
        public int? DeliveredTo { get; set; }
        public int StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? TotalTimeAllowed { get; set; }
        public int? TotalTimeAllowedId { get; set; }
        public decimal? Budget { get; set; }
        public int? Currency { get; set; }
        public int? Baths { get; set; }
        public int? Beds { get; set; }
        public int? FamilyMembers { get; set; }
        public int? PaymentResponsibility { get; set; }
        public int? SupplierPartner { get; set; }
        public int? MainContact { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ScheduledPickUpDate { get; set; }
        public int? TotalDays { get; set; }
        public int? DeliverTo { get; set; }
        public string PropertyAddress { get; set; }
        public decimal? RentalAmount { get; set; }
        public int? CurrencyExtension { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public decimal? PaymentDue { get; set; }
        public int? ReturnSecurityDeposit { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? IdCatPaymentmethods { get; set; }
        public decimal? RentalOngoingAmount { get; set; }

        public virtual CatCurrencyDto CurrencyExtensionNavigation { get; set; }
        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        public virtual CatSupplierDto SupplierPartnerNavigation { get; set; }
        public virtual CatDurationDto TotalTimeAllowedNavigation { get; set; } 
        public virtual ICollection<CommentRentalFurnitureCoordinationSelectDto> CommentRentalFurnitureCoordinations { get; set; }
        public virtual ICollection<DocumentRentalFurnitureCoordinationDto> DocumentRentalFurnitureCoordinations { get; set; }
        public virtual ICollection<ExtensionRentalFurnitureCoordinationDto> ExtensionRentalFurnitureCoordinations { get; set; }
        public virtual ICollection<PaymentsRentalFurnitureCoordinationDto> PaymentsRentalFurnitureCoordinations { get; set; }
        public virtual ICollection<ReminderRentalFurnitureCoordinationDto> ReminderRentalFurnitureCoordinations { get; set; }
        public virtual ICollection<StayExtensionRentalFurnitureCoordinationDto> StayExtensionRentalFurnitureCoordinations { get; set; }

        public virtual ICollection<PaymentRentalDto> PaymentRentals { get; set; }
    }
}
