using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class LifeCircleRepository :GenericRepository<biz.premier.Entities.LifeCircle>, ILifeCircleRepository
    {
        public LifeCircleRepository(Db_PremierContext context) : base(context) { }
    }
}
