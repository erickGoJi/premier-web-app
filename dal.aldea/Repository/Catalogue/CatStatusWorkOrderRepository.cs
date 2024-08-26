using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CatStatusWorkOrderRepository : GenericRepository<CatStatusWorkOrder>, ICatStatusWorkOrderRepository
    {
        public CatStatusWorkOrderRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}