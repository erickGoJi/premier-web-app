using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class TimeZoneRepository : GenericRepository<biz.premier.Entities.CatTimeZone>, ITimeZoneRepository
    {
        public TimeZoneRepository(Db_PremierContext context) : base(context) { }
    }
}
