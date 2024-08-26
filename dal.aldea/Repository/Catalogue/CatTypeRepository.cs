using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CatTypeRepository: GenericRepository<CatType>, ICatTypeRepository
    {
        public CatTypeRepository(Db_PremierContext context):base(context){}
    }
}