using biz.premier.Repository.Country;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Country
{
    public class CityAttractionsRepository : GenericRepository<biz.premier.Entities.CityAttraction>, ICityAttractionsRepository
    {
        public CityAttractionsRepository(Db_PremierContext context) : base(context)
        {
        }
    }
}
