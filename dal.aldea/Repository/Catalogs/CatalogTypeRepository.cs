using biz.premier.Repository.Catalogs;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogs
{
    public class CatalogTypeRepository : GenericRepository<biz.premier.Entities.CatTypeCatalog>, ICatalogTypeRepository
    {
        public CatalogTypeRepository(Db_PremierContext context):base(context){}
    }
}