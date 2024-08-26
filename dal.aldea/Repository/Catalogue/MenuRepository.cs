using System.Collections.Generic;
using System.Linq;
using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.Catalogue
{
    public class MenuRepository : GenericRepository<CatMenu>, IMenuRepository
    {
        public MenuRepository(Db_PremierContext context):base(context){}
        
        public List<CatMenu> GetMenus()
        {
            var menus = _context.CatMenus
                .Include(i => i.CatSubMenus)
                .ThenInclude(x => x.CatSections)
                .ToList();
            return menus;
        }
    }
}