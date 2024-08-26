using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileService
{
    public class SupplierPartnerDetailDto
    {
        public int Id { get; set; }
        public int? SupplierPartnerProfileService { get; set; }
        public int? FleetSize { get; set; }
        public int? NumberDirvers { get; set; }
        public bool? ArmouredVehicles { get; set; }
        public int? TypeVehicles { get; set; }
        public bool? CarSeatRental { get; set; }
        public string LicenceMlsNumber { get; set; }
        public bool? InsuranceCoverage { get; set; }
        public bool? OnlineSolution { get; set; }
        public bool? FaceFaceAssistance { get; set; }
        public bool? PetFriendly { get; set; }
        public bool? AirportShuttle { get; set; }
        public bool? MealPlans { get; set; }
        public bool? MergencyKits { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Ib { get; set; }
        public bool? Uniform { get; set; }
        public bool? Medical { get; set; }
        public bool? TransportationOffered { get; set; }
        public string Address { get; set; }
        public int? Grade { get; set; }
        public int? Language { get; set; }
        public string WebSite { get; set; }

        public virtual SupplierPartnerProfileServiceDto SupplierPartnerProfileServiceNavigation { get; set; }
        public virtual ICollection<TypeVehiclesSupplierPartnerDetailDto> TypeVehiclesSupplierPartnerDetails { get; set; }

    }
}
