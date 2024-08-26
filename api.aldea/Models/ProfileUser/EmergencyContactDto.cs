using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ProfileUser
{
    public class EmergencyContactDto
    {
        public int Id { get; set; }
        public int? Profile { get; set; }
        public string ContactName { get; set; }
        public int? Relationship { get; set; }
        public string PhoneNumber { get; set; }
        public string HomeNumber { get; set; }
        public string Location { get; set; }
        
        public virtual CatRelationshipContactDto RelationshipNavigation { get; set; }
    }
    public class EmergencyContactSelectDto
    {
        public int Id { get; set; }
        public int? Profile { get; set; }
        public string ContactName { get; set; }
        public int? Relationship { get; set; }
        public string PhoneNumber { get; set; }
        public string HomeNumber { get; set; }
        public string Location { get; set; }

        public virtual PersonalInformationDto ProfileNavigation { get; set; }
        public virtual CatRelationshipContactDto RelationshipNavigation { get; set; }
    }
}
