using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class DayRepository : GenericRepository<biz.premier.Entities.CatDay>, IDayRepository
    {
        public DayRepository(Db_PremierContext context) : base(context) { }
    }
}
