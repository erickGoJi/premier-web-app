using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CatIndustryRepository : GenericRepository<CatIndustry>, ICatIndustryRepository
    {
        public CatIndustryRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}