using biz.premier.Entities;
using biz.premier.Repository.TenancyManagement;
using dal.premier.DBContext;

namespace dal.premier.Repository.TenancyManagement
{
    public class CatStatusReportAnEventRepository : GenericRepository<CatStatusReportAnEvent>, ICatStatusReportAnEventRepository
    {
        public CatStatusReportAnEventRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}