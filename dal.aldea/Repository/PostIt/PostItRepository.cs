using biz.premier.Repository.PostIt;
using dal.premier.DBContext;

namespace dal.premier.Repository.PostIt
{
    public class PostItRepository : GenericRepository<biz.premier.Entities.PostIt>, IPostItRepository
    {
        public PostItRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}