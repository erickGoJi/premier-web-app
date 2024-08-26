using api.premier.Models.Permit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Catalogos
{
    public class CatRoleDto
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Status { get; set; }
        
        public virtual ICollection<PermissionDto> Permissions { get; set; }
    }
}
