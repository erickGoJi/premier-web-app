using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Departure
{
    public class DepartureRepairDto
    {
        public int Id { get; set; }
        public int? DepartaureId { get; set; }
        public int? RepairType { get; set; }
        public int? SupplierPartner { get; set; }
        public DateTime? RepairStartDate { get; set; }
        public DateTime? RepairEndDate { get; set; }
        public int? TotalDays { get; set; }
        public string Comments { get; set; }

        public virtual DepartureDto Departaure { get; set; }
        public virtual CatRepairTypeDto RepairTypeNavigation { get; set; }
        public virtual ICollection<DocumentRepairDto> DocumentRepairs { get; set; }
    }
}
