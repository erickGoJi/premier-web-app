using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CatLibreryRepository : GenericRepository<CatLibrary>, ICatLibreryRepository
    {
        public CatLibreryRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}