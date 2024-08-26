using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.Models.SupplierPartnerProfileConsultant;

namespace api.premier.Models.ImmigrationCoodinator
{
    public class ImmigrationCoodinatorDto
    {
        public int Id { get; set; }
        public int? CoordinatorTypeId { get; set; }
        public int? CoordinatorId { get; set; }
        public DateTime? Assigned { get; set; }
        public DateTime? Accepted { get; set; }
        public int ServiceRecordId { get; set; }
        public int? StatusId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class ImmigrationCoodinatorSelectDto 
    {
        public int Id { get; set; }
        public int? CoordinatorTypeId { get; set; }
        public int? CoordinatorId { get; set; }
        public DateTime? Assigned { get; set; }
        public DateTime? Accepted { get; set; }
        public int ServiceRecordId { get; set; }
        public int? StatusId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ProfileUserDto Coordinator { get; set; }
        public virtual CatStatusSupplierCoordinatorDto Status { get; set; }
    }
}
