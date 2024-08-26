using api.premier.Models.Catalogos;
using api.premier.Models.ServiceOrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AirportTransportationService
{
    public class AirportTransportationServiceDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public int? ApplicantId { get; set; }
        public bool? Coordination { get; set; }
        public int StatusId { get; set; }
        public string ProjectFee { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public decimal? Budget { get; set; }
        public int? TotalTimeAllowed { get; set; }
        public int? TotalTimeAllowedId { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? TransportType { get; set; }
        public DateTime? ServiceDate { get; set; }
        public string TimeServicesHour { get; set; }
        public string TimeServicesMinute { get; set; }
        public string FlightNumber { get; set; }
        public string PickUpLocation { get; set; }
        public string DropOffLocation { get; set; }
        public int? NumberLuggage { get; set; }
        public int? SupplierPartner { get; set; }
        public string DriverName { get; set; }
        public string DriverContact { get; set; }
        public string Vehicle { get; set; }
        public string VehicleColor { get; set; }
        public string PlateNumber { get; set; }
        public bool? Pet { get; set; }
        public DateTime? PickupTime { get; set; }
        public decimal? CostService { get; set; }
        public int? PaymentResponsibility { get; set; }
        public int? IdCatPaymentmethods { get; set; }
        public int? IdCurrency { get; set; }

        public virtual ICollection<CommentAirportTransportationServiceDto> CommentAirportTransportationServices { get; set; }
        public virtual ICollection<DocumentAirportTransportationServiceDto> DocumentAirportTransportationServices { get; set; }
        public virtual ICollection<PaymentAirportTransportationServiceDto> PaymentAirportTransportationServices { get; set; }
        public virtual ICollection<ReminderAirportTransportationServiceDto> ReminderAirportTransportationServices { get; set; }
        public virtual ICollection<AirportTransportPickupDto> AirportTransportPickup { get; set; }

    }

    public class CreateAirportTransportationDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime? TpAuthoDate { get; set; }
        public DateTime? TpAuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
        public int StatusId { get; set; }
        public int? TotalTimeAllowed { get; set; }
        public int? TotalTimeAllowedId { get; set; }
        public decimal? Budget { get; set; }

        public virtual ICollection<CommentAirportTransportationServiceDto> CommentAirportTransportationServices { get; set; }
        public virtual ICollection<DocumentAirportTransportationServiceDto> DocumentAirportTransportationServices { get; set; }
        public virtual ICollection<PaymentAirportTransportationServiceDto> PaymentAirportTransportationServices { get; set; }
        public virtual ICollection<ReminderAirportTransportationServiceDto> ReminderAirportTransportationServices { get; set; }
        public virtual ICollection<AirportTransportPickupDto> AirportTransportPickup { get; set; }
    }
        public class AirportTransportationServiceSelectDto
        {
            public int Id { get; set; }
            public int WorkOrderServicesId { get; set; }
            public DateTime AuthoDate { get; set; }
            public DateTime AuthoAcceptanceDate { get; set; }
            public int? ApplicantId { get; set; }
            public bool? Coordination { get; set; }
            public int StatusId { get; set; }
            public DateTime ServiceCompletionDate { get; set; }
            public string Comments { get; set; }
            public int? CreatedBy { get; set; }
            public DateTime? CreatedDate { get; set; }
            public int? UpdateBy { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public int? TransportType { get; set; }
            public DateTime? ServiceDate { get; set; }
            public string TimeServicesHour { get; set; }
            public string TimeServicesMinute { get; set; }
            public string FlightNumber { get; set; }
            public string PickUpLocation { get; set; }
            public string DropOffLocation { get; set; }
            public int? NumberLuggage { get; set; }
            public int? SupplierPartner { get; set; }
            public string DriverName { get; set; }
            public string DriverContact { get; set; }
            public string Vehicle { get; set; }
            public string VehicleColor { get; set; }
            public string PlateNumber { get; set; }
            public bool? Pet { get; set; }
            public virtual ICollection<CommentAirportTransportationServiceSelectDto> CommentAirportTransportationServices { get; set; }
            public virtual ICollection<DocumentAirportTransportationServiceDto> DocumentAirportTransportationServices { get; set; }
            public virtual ICollection<ExtensionAirportTransportationServiceDto> ExtensionAirportTransportationServices { get; set; }
            public virtual ICollection<PaymentAirportTransportationServiceDto> PaymentAirportTransportationServices { get; set; }
            public virtual ICollection<ReminderAirportTransportationServiceDto> ReminderAirportTransportationServices { get; set; }
            public virtual ICollection<FamilyMemberTransportServiceDto> FamilyMemberTransportServices { get; set; }
        }
    }

