using System;
using biz.premier.Entities;

namespace biz.premier.Repository
{
    public interface IUpcomingEventRepository : IGenericRepository<UpcomingEvent>
    {
        Tuple<int, int> GetRegion(int user);
    }
}