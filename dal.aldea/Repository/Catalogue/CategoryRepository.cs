using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class CategoryRepository : GenericRepository<CatCategory>, ICategoryRepository
    {
        public CategoryRepository(Db_PremierContext context) : base(context)
        {

        }

        public ActionResult GetCataegoryByServiceLineIdWhitNickName(int serviceLine)
        {
            var services = _context.ServiceLocations
                .Where(x => x.IdServiceLine == serviceLine)
                .Select(s => new
                {
                    id = s.IdServiceNavigation.Category.Id,
                    category = s.IdServiceNavigation.Category.Category,
                    sericeLineId = s.IdServiceNavigation.Category.SericeLineId,
                    //id = s.IdServiceNavigation.Id,
                    service = s.NickName,
                    //s.IdServiceNavigation.CategoryId
                }).OrderBy(x => x.service).ToList();
            return new ObjectResult(services);
        }
    }
}
