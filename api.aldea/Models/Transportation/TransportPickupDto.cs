using System;
using System.Collections.Generic;

namespace api.premier.Models.Transportation
{
    public class TransportPickupDto
    {
        public int Id { get; set; }
        public int? TransportationId { get; set; }
        public int? TpStatusId { get; set; }
        public int? TpTransportType { get; set; }
        public DateTime? TpServiceDate { get; set; }
        public string TpTimeServicesHour { get; set; }
        public string TpTimeServicesMinute { get; set; }
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
        public virtual ICollection<FamilyMemberTransportationDto> FamilyMemberTransportations { get; set; }
    }
}