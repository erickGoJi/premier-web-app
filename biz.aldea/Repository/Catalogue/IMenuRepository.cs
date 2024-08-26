using System.Collections.Generic;
using biz.premier.Entities;

namespace biz.premier.Repository.Catalogue
{
    public interface IMenuRepository : IGenericRepository<CatMenu>
    {
        List<CatMenu> GetMenus();
    }
}