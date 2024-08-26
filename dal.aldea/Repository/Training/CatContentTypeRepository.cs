using biz.premier.Entities;
using biz.premier.Repository.Training;
using dal.premier.DBContext;

namespace dal.premier.Repository.Training
{
    public class CatContentTypeRepository : GenericRepository<CatContentType>, ICatContentTypeRepository
    {
        public CatContentTypeRepository(Db_PremierContext context):base(context){}
    }
}