using biz.premier.Entities;
using biz.premier.Repository.Training;
using dal.premier.DBContext;

namespace dal.premier.Repository.Training
{
    public class CatElementRepository : GenericRepository<CatElement>, ICatElementRepository
    {
        public CatElementRepository(Db_PremierContext context):base(context){}
    }
}