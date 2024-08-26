using System;

namespace api.premier.Models.SupplierPartnerProfileService
{
    public class CampusDto
    {
        public int Id { get; set; }
        public int AreaCoverageId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}