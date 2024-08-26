using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CatStatusReportIssueRepository : GenericRepository<CatStatusReportIssue>, ICatStatusReportIssueRepository
    {
        public CatStatusReportIssueRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}