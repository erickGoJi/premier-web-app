using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CatPaymentRecurrenceRepository : GenericRepository<CatPaymentRecurrence>, ICatPaymentRecurrenceRepository
    {
        public CatPaymentRecurrenceRepository(Db_PremierContext context):base(context){}
    }
}