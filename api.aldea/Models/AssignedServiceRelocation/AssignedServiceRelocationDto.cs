using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AssignedServiceRelocation
{
    public class AssignedServiceRelocationDto
    {
        public int Id { get; set; }
        public int? StandaloneServiceId { get; set; }
        public int? PackageServiceId { get; set; }
        public int? RelocationSupplierPartnerId { get; set; }
    }
}
