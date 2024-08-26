using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace dal.premier.Repository.Catalogue
{
    public class RoleRepository : GenericRepository<CatRole>, IRoleRepository
    {
        public RoleRepository(Db_PremierContext context) : base(context) { }

        public ActionResult GetCustom(int key)
        {
            if (key == 0)
                return null;
            var role = _context.Set<biz.premier.Entities.CatRole>()
                .Include(i => i.Permissions)
                .Select(c => new
                {
                    c.Id,
                    c.Role,
                    c.Description,
                    c.CreatedBy,
                    c.CreatedDate,
                    c.UpdateBy,
                    c.UpdatedDate,
                    c.Status,
                    Permissions = c.Permissions.Select(x => new
                    {
                        x.Id,
                        menu = x.IdCatMenuNavigation.Name,
                        submenu = x.IdCatSubMenuNavigation.Name,
                        seccion = x.IdCatSeccionNavigation.Name,
                        x.IdCatMenu,
                        x.Role,
                        x.IdCatSubMenu,
                        x.IdCatSeccion,
                        x.Reading,
                        x.Writing,
                        x.Editing,
                        x.Deleting
                    })
                }).Where(s => s.Id == key);



            return new ObjectResult(role);
        }
        public CatRole UpdateCustom(CatRole dto, int key)
        {
            if (dto == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.CatRole>()
                .Include(i => i.Permissions)
                .SingleOrDefault(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(dto);
                foreach (var permission in dto.Permissions)
                {
                    var @default = exist.Permissions.SingleOrDefault(x => x.Id == permission.Id);
                    if (@default == null)
                    {
                        exist.Permissions.Add(permission);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(@default).CurrentValues.SetValues(permission);
                        //foreach (var permissionSection in permission.Sections)
                        //{
                        //    var section = @default.Sections.SingleOrDefault(x => x.Id == permissionSection.Id);
                        //    if (section == null)
                        //    {
                        //        @default.Sections.Add(permissionSection);
                        //        _context.SaveChanges();
                        //    }
                        //    else
                        //    {
                        //        _context.Entry(section).CurrentValues.SetValues(permissionSection);
                        //    }
                        //}
                        //foreach (var subMenu in permission.PermissionSubMenus)
                        //{
                        //    var permissionSubMenu = @default.PermissionSubMenus.SingleOrDefault(x => x.Id == subMenu.Id);
                        //    if (permissionSubMenu == null)
                        //    {
                        //        @default.PermissionSubMenus.Add(subMenu);
                        //        _context.SaveChanges();
                        //    }
                        //    else
                        //    {
                        //        _context.Entry(permissionSubMenu).CurrentValues.SetValues(subMenu);
                        //    }
                        //}
                    }

                }
                _context.SaveChanges();
            }
            return exist;
        }

        public ActionResult GetSection(int menu, int submenu, int role)
        {
            var find = _context.Permissions.Where(x => x.IdCatMenu == menu && x.IdCatSubMenu == submenu && x.Role == role).ToList();
            return new ObjectResult(find);
        }
    }
}
