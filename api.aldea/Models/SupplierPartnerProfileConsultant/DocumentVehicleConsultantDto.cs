using api.premier.Models.Catalogos;
using api.premier.Models.Catalogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileConsultant
{
    public class DocumentVehicleConsultantDto
    {
        public int Id { get; set; }
        public int? VehicleConsultant { get; set; }
        public int? City { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public int? DocumentType { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? Privacy { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatCityDto CityNavigation { get; set; }
        public virtual CatDocumentTypeDto DocumentTypeNavigation { get; set; }
        public virtual CatPrivacyDto PrivacyNavigation { get; set; }
        public virtual VehicleConsultantDto VehicleConsultantNavigation { get; set; }
    }
}
