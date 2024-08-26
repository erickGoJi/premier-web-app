using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Permit
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public int? Role { get; set; }
        public int? IdCatMenu { get; set; }
        public int? IdCatSubMenu { get; set; }
        public int? IdCatSeccion { get; set; }
        public bool? Reading { get; set; }
        public bool? Writing { get; set; }
        public bool? Editing { get; set; }
        public bool? Deleting { get; set; }
        public bool? Show { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
    
    public class CatMenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<CatSubMenuDto> CatSubMenus { get; set; }
        public virtual ICollection<PermissionDto> Permissions { get; set; }
    }
    
    public class CatSubMenuDto
    {
        public int Id { get; set; }
        public int? Menu { get; set; }
        public string Name { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatMenuDto MenuNavigation { get; set; }
    }

    public class CatSectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SubMenu { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatSubMenuDto SubMenuNavigation { get; set; }
        public virtual ICollection<PermissionDto> Permissions { get; set; }
    }
}
