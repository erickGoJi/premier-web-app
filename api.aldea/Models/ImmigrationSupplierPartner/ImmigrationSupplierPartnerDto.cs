using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ImmigrationSupplierPartner
{
    public class ImmigrationSupplierPartnerDto
    {
        public int Id { get; set; }
        public int? SupplierTypeId { get; set; }
        public int? SupplierCompanyId { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public int ServiceRecordId { get; set; }
        public int? SupplierId { get; set; }
        public int? StatusId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
