using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace biz.premier.Repository.Catalogue
{
    public interface IRoleRepository : IGenericRepository<CatRole>
    {
        ActionResult GetCustom(int key);
        CatRole UpdateCustom(CatRole dto, int key);
        ActionResult GetSection(int menu, int submenu, int role);
    }
}
