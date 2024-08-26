using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CatStatusSaleRepository : GenericRepository<CatStatusSale>, ICatStatusSaleRepository
    {
        public CatStatusSaleRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}