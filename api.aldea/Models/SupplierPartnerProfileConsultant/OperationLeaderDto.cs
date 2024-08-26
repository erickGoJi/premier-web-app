using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileConsultant
{
    public class OperationLeaderDto
    {
        public int CreatedBy { get; set; }
        public int Consultant { get; set; }
        public virtual ProfileUserDto ConsultantNavigation { get; set; }
        public virtual ProfileUserDto CreatedByNavigation { get; set; }
    }
}
