using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class EventRepository : GenericRepository<CatEvent>, IEventRepository
    {
        public EventRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}