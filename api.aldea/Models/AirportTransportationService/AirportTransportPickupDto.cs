using System;
using System.Collections.Generic;

namespace api.premier.Models.AirportTransportationService
{
    public class AirportTransportPickupDto
    {
        public int Id { get; set; }
        public int? TransportationId { get; set; }
        public DateTime TpAuthoDate { get; set; }
        public DateTime TpAuthoAcceptanceDate { get; set; }
        public int? TpStatusId { get; set; }
        public int? TpTransportType { get; set; }
        public DateTime? TpServiceDate { get; set; }
        public string TpTimeServicesHour { get; set; }
        public string TpTimeServicesMinute { get; set; }
        public string TpFlightNumber { get; set; }
        public DateTime? TpServiceCompletionDate { get; set; }
        public int? TpProjectFee { get; set; }
        public string TpPickUpLocation { get; set; }
        public string TpDropOffLocation { get; set; }
        public int? TpSupplierPartner { get; set; }
        public string TpDriverName { get; set; }
        public string TpDriverContact { get; set; }
        public string TpVehicle { get; set; }
        public string TpPlateNumber { get; set; }
        public string TpVehicleColor { get; set; }
        public bool? TpPet { get; set; }

        public virtual ICollection<FamilyMemberTransportServiceDto> FamilyMemberTransportServices { get; set; }
    }
}