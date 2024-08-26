using biz.premier.Repository.Country;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Country
{
    public class CityWhereEatRepository : GenericRepository<biz.premier.Entities.CityWhereEat>, ICityWhereEatRepository
    {
        public CityWhereEatRepository(Db_PremierContext context) : base(context)
        {
        }
    }
}
