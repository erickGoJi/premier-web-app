using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Transportation
{
    public class TransportationDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public int? ApplicantId { get; set; }
        public bool? Coordination { get; set; }
        public int StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? TotalTimeAllowed { get; set; }
        public int? TotalTimeAllowedId { get; set; }
        public decimal? Budget { get; set; }
        public string Comments { get; set; }
        public string ProjectFee { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? TransportType { get; set; }
        public DateTime? ServiceDate { get; set; }
        public string TimeServicesHour { get; set; }
        public string TimeServicesMinute { get; set; }
        public string PickUpLocation { get; set; }
        public string DropOffLocation { get; set; }
        public int? SupplierPartner { get; set; }
        public string DriverName { get; set; }
        public string DriverContact { get; set; }
        public string Vehicle { get; set; }
        public string PlateNumber { get; set; }
        public string VehicleColor { get; set; }
        public bool? Pet { get; set; }

        public virtual ICollection<CommentTransportationDto> CommentTransportations { get; set; }
        public virtual ICollection<DocumentTransportationDto> DocumentTransportations { get; set; }
        public virtual ICollection<PaymentTransportationDto> PaymentTransportations { get; set; }
        public virtual ICollection<ReminderTransportationDto> ReminderTransportations { get; set; }
        public virtual ICollection<FamilyMemberTransportationDto> FamilyMemberTransportations { get; set; }
        public virtual ICollection<TransportPickupDto> TransportPickups { get; set; }

    }

    public class CreateTransportationDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
        public int StatusId { get; set; }
        public int? TotalTimeAllowed { get; set; }
        public int? TotalTimeAllowedId { get; set; }
        public decimal? Budget { get; set; }

        public virtual ICollection<CommentTransportationDto> CommentTransportations { get; set; }
        public virtual ICollection<DocumentTransportationDto> DocumentTransportations { get; set; }
        public virtual ICollection<PaymentTransportationDto> PaymentTransportations { get; set; }
        public virtual ICollection<ReminderTransportationDto> ReminderTransportations { get; set; }
        public virtual ICollection<TransportPickupDto> TransportPickups { get; set; }

    }

    public class TransportationSelectDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public int? ApplicantId { get; set; }
        public bool? Coordination { get; set; }
        public int StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? TotalTimeAllowed { get; set; }
        public int? TotalTimeAllowedId { get; set; }
        public decimal? Budget { get; set; }
        public string Comments { get; set; }
        public string ProjectFee { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? TransportType { get; set; }
        public DateTime? ServiceDate { get; set; }
        public string TimeServicesHour { get; set; }
        public string TimeServicesMinute { get; set; }
        public string PickUpLocation { get; set; }
        public string DropOffLocation { get; set; }
        public int? SupplierPartner { get; set; }
        public string DriverName { get; set; }
        public string DriverContact { get; set; }
        public string Vehicle { get; set; }
        public string PlateNumber { get; set; }
        public string VehicleColor { get; set; }
        public bool? Pet { get; set; }

        public virtual CatStatusDto Status { get; set; }
        public virtual CatDurationDto TotalTimeAllowedNavigation { get; set; }
        public virtual ICollection<CommentTransportationSelectDto> CommentTransportations { get; set; }
        public virtual ICollection<DocumentTransportationDto> DocumentTransportations { get; set; }
        public virtual ICollection<ExtensionTransportationDto> ExtensionTransportations { get; set; }
        public virtual ICollection<PaymentTransportationDto> PaymentTransportations { get; set; }
        public virtual ICollection<ReminderTransportationDto> ReminderTransportations { get; set; }
        public virtual ICollection<FamilyMemberTransportationSelectDto> FamilyMemberTransportations { get; set; }
        public virtual ICollection<TransportPickupDto> TransportPickups { get; set; }
    }
}
