using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AirportTransportationService
{
    public class TransportServiceDto
    {
        public int Id { get; set; }
        public int? AirportTransportationServicesId { get; set; }
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
        public string PlateNumber { get; set; }
        public bool? Pet { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AirportTransportationServiceDto AirportTransportationServices { get; set; }
        //public virtual CatTransportType AirportTransportationServicesNavigation { get; set; }
        //public virtual CatSupplier SupplierPartnerNavigation { get; set; }
        public virtual ICollection<FamilyMemberTransportServiceDto> FamilyMemberTransportServices { get; set; }
    }
}
