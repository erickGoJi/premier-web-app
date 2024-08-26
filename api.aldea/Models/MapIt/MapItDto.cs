using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.MapIt
{
    public class MapItDto
    {
        public int Id { get; set; }
        public int? ServiceRecord { get; set; }
        public int? SupplierPartner { get; set; }
        public int? ServiceLine { get; set; }
        public DateTime? StartDate { get; set; }
        public string DriverName { get; set; }
        public string DriverContact { get; set; }
        public string Vehicle { get; set; }
        public string PlateNumber { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual ServiceRecord ServiceRecordNavigation { get; set; }
        //public virtual CatSupplier SupplierPartnerNavigation { get; set; }
        public virtual ICollection<LocationDto> Locations { get; set; }
    }
}
