using api.premier.Models.AssigneeInformation;
using api.premier.Models.Catalogos;
using api.premier.Models.SupplierPartnerProfileConsultant;
using System;
using System.Collections.Generic;

namespace api.premier.Models
{
    public class UserDto
    {
         public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string MotherLastName { get; set; }
        public string MobilePhone { get; set; }
        public string Avatar { get; set; }
        public bool? Reset { get; set; }
        public string token { get; set; }
        public bool Status { get; set; }
        public string ClientName { get; set; }
        public virtual CatUserTypeDto UserType { get; set; }
        public virtual CatRoleDto Role { get; set; }
        public virtual ICollection<AssigneeInformationDto> AssigneeInformations { get; set; }
        public virtual ICollection<ProfileUserDto> ProfileUsers { get; set; }


    }
}
