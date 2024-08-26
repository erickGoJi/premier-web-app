using biz.premier.Repository.Country;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Country
{
    public class CityEmergencyRepository : GenericRepository<biz.premier.Entities.CityEmergency>, ICityEmergencyRepository
    {
        public CityEmergencyRepository(Db_PremierContext context) : base(context)
        {
        }
    }
}
