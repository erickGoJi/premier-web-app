using System;
using System.Linq;
using biz.premier.Entities;
using biz.premier.Repository;
using dal.premier.DBContext;

namespace dal.premier.Repository
{
    public class UpcomingEventRepository : GenericRepository<UpcomingEvent>, IUpcomingEventRepository
    {
        public UpcomingEventRepository(Db_PremierContext context):base(context){}
        public Tuple<int, int> GetRegion(int user)
        {
            var profile = _context.ProfileUsers.Where(x => x.UserId == user)
                .Select(s => new Tuple<int, int>(s.Country.Value, s.City.Value)).FirstOrDefault();
            return profile;
        }
    }
}