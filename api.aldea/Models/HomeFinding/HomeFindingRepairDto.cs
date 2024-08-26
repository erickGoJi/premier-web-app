using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class HomeFindingRepairDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public int? RepairType { get; set; }
        public int? SupplierPartner { get; set; }
        public DateTime? RepairStartDate { get; set; }
        public DateTime? RepairEndDate { get; set; }
        public int? TotalDays { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual HomeFindingDto HomeFinding { get; set; }
        public virtual CatRepairTypeDto RepairTypeNavigation { get; set; }
        public virtual CatSupplierCompanyDto SupplierPartnerNavigation { get; set; }
        public virtual ICollection<DocumentRepairHomeFindingDto> DocumentRepairHomeFindings { get; set; }
    }
}
