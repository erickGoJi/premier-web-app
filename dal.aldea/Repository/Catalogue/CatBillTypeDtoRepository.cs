using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CatBillTypeDtoRepository : GenericRepository<CatBillType>, ICatBillTypeDtoRepository
    {
        public CatBillTypeDtoRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}