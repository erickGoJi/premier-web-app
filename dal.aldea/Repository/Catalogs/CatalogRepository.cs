using biz.premier.Repository.Catalogs;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogs
{
    public class CatalogRepository : GenericRepository<biz.premier.Entities.CatCatalog>, ICatalogRepository
    {
        public CatalogRepository(Db_PremierContext context): base(context){}
    }
}