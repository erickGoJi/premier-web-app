using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.Models.SupplierPartnerProfileConsultant;

namespace api.premier.Models.ProfileUser
{
    public class AssignedTeamDto
    {
        public int AssignedBy { get; set; }
        public int AssignedTo { get; set; }
        public virtual ProfileUserDto AssignedByNavigation { get; set; }
        public virtual ProfileUserDto AssignedToNavigation { get; set; }
    }
}
