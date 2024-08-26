using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CatSeverityDtoRepository : GenericRepository<CatSeverity>, ICatSeverityDtoRepository
    {
        public CatSeverityDtoRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}