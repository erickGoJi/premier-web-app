using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CatStatusHomePurchaseRepository : GenericRepository<CatStatusHomePurchase>, ICatStatusHomePurchaseRepository
    {
        public CatStatusHomePurchaseRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}