using api.premier.Models.DependentInformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Transportation
{
    public class FamilyMemberTransportationDto
    {
        public int TransportService { get; set; }
        public int FamilyMember { get; set; }
    }
    public class FamilyMemberTransportationSelectDto
    {
        public int TransportService { get; set; }
        public int FamilyMember { get; set; }
        public virtual DependentInformationDto FamilyMemberNavigation { get; set; }
    }
}
