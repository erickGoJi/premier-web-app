using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class RelationshipRepository : GenericRepository<CatRelationship>, IRelationshipRepository
    {
        public RelationshipRepository(Db_PremierContext context): base(context) { }

        public ActionResult GetCustomRelationShip()
        {
            var role = _context.Set<biz.premier.Entities.CatRelationship>()
                .Select(c => new
                {
                    c.Id,
                    c.Relationship
                }).Where(s => s.Id != 7).OrderBy(x => x.Relationship);

            return new ObjectResult(role);
        }
    }
}
